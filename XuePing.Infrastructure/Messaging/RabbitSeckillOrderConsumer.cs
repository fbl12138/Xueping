using Microsoft.Extensions.DependencyInjection;                    // IServiceScopeFactory
using Microsoft.Extensions.Logging;                                // ILogger
using RabbitMQ.Client;                                             // IChannel / BasicProperties / BasicGetResult
using System.Collections.Generic;                                  // Dictionary：复制 x-death 头
using System.Text;                                                 // Encoding：解析 x-death 里的 byte[] 头
using System.Text.Json;                                            // 反序列化消息体
using XuePing.Application.Services;                                // ISeckillOrderConsumer / ISeckillOrderHandler
using XuePing.Domain.Common;                                       // SeckillOrderMessage

namespace XuePing.Infrastructure.Messaging
{
    public sealed class RabbitSeckillOrderConsumer : ISeckillOrderConsumer, IAsyncDisposable
    {
        private const int MaxAttempt = 3;                                            // 最多处理 3 次，含第一次
        private readonly RabbitMqConnectionProvider _provider;                        // 单例连接
        private readonly IServiceScopeFactory _factory;                               // 每条消息开独立作用域
        private readonly ILogger<RabbitSeckillOrderConsumer> _logger;                 // 日志
        private IChannel? _channel;                                                   // 复用的信道

        public RabbitSeckillOrderConsumer(RabbitMqConnectionProvider provider,
            IServiceScopeFactory factory, ILogger<RabbitSeckillOrderConsumer> logger)
        {
            _provider = provider;
            _factory = factory;
            _logger = logger;
        }

        private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default)
        {
            if (_channel is { IsOpen: true }) return _channel;                        // 信道还活着就复用
            var connection = await _provider.GetAsync(cancellationToken);             // 确保 TCP 连接
            _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken); // 普通信道
            await RabbitOrderTopology.DeclareAsync(_channel, cancellationToken);      // 声明拓扑（幂等，参数必须与上次一致）
            return _channel;
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null) await _channel.DisposeAsync();                  // 关信道
        }

        public async Task ConsumeOnceAsync(CancellationToken cancellationToken = default)
        {
            var channel = await GetChannelAsync(cancellationToken);                   // 拿信道
            var result = await channel.BasicGetAsync(                                 // 拉一条
                RabbitOrderTopology.QueueName,
                autoAck: false,                                                       // 手动确认
                cancellationToken: cancellationToken);
            if (result is null) return;                                               // 空队列，返回交给 Worker 节流

            SeckillOrderMessage? order = null;
            try
            {
                order = JsonSerializer.Deserialize<SeckillOrderMessage>(result.Body.Span); // 反序列化
                if (order is null)                                                    // 脏消息
                {
                    await channel.BasicAckAsync(result.DeliveryTag, false, cancellationToken); // 直接确认丢弃
                    return;
                }

                using var scope = _factory.CreateScope();                              // 独立作用域拿 scoped handler
                var handler = scope.ServiceProvider.GetRequiredService<ISeckillOrderHandler>();
                await handler.HandleAsync(order, cancellationToken);                   // 幂等入库
                await channel.BasicAckAsync(result.DeliveryTag, false, cancellationToken); // 先入库成功，再 ACK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ 消费抢课失败 deliveryTag={Tag}", result.DeliveryTag);

                var attempt = CountRejections(result.BasicProperties) + 1;             // x-death 里的 rejected 次数 + 本次
                if (attempt < MaxAttempt)                                              // 还没到上限
                {
                    await channel.BasicNackAsync(                                     // 拒收且不重回队列
                        result.DeliveryTag, multiple: false, requeue: false,
                        cancellationToken: cancellationToken);                          // → 走 DLX 进重试队列，3 秒后自动回来
                    return;                                                           // 状态先不写，等终死再写
                }

                // 转投终死队列前复制一份可写属性：泛型发布只接受 IAmqpHeader 实现，接收到的属性是只读的
                var deadProps = new BasicProperties
                {
                    DeliveryMode = result.BasicProperties.DeliveryMode,
                    Headers = result.BasicProperties.Headers is null
                        ? null
                        : new Dictionary<string, object?>(result.BasicProperties.Headers)
                };
                await channel.BasicPublishAsync(                                       // 超限：自己转投终死队列
                    exchange: RabbitOrderTopology.RetryExchange,
                    routingKey: RabbitOrderTopology.DeadRoutingKey,
                    mandatory: true,                                                   // 路由不到要报错
                    basicProperties: deadProps,                                        // 沿用原属性，保住 x-death 历史
                    body: result.Body,                                                 // 原样转发消息体
                    cancellationToken: cancellationToken);

                await channel.BasicAckAsync(result.DeliveryTag, false, cancellationToken); // 转投成功才确认原消息

                if (order is not null)                                                // 能拿到 userId 才回写状态
                {
                    using var scope = _factory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<ISeckillOrderHandler>();
                    await handler.MarkFailedAsync(order, "系统繁忙，请稍后重试", cancellationToken);
                }
            }
        }

        /// <summary>从 x-death 里数"因被拒而死在主队列"的次数</summary>
        private static int CountRejections(IReadOnlyBasicProperties? props)
        {
            if (props?.Headers is null) return 0;                                      // 没有头 → 首次投递
            if (!props.Headers.TryGetValue("x-death", out var raw)) return 0;          // 没有死亡记录 → 首次投递
            if (raw is not IList<object?> records) return 0;                           // 结构不符，按首次处理

            foreach (var item in records)                                              // 每条记录对应一种死因
            {
                if (item is not IDictionary<string, object?> table) continue;          // 结构不符，跳过

                var queue = ReadString(table, "queue");                                // 哪个队列死的
                var reason = ReadString(table, "reason");                              // 死因
                if (queue != RabbitOrderTopology.QueueName) continue;                  // 不是主队列的死亡记录，跳过
                if (reason != "rejected") continue;                                    // 不是被拒（expired 是 TTL 造成的），跳过
                return table.TryGetValue("count", out var c) && c is long count      // count 是 long
                    ? (int)count
                    : 0;
            }

            return 0;                                                                  // 没匹配到 → 首次投递
        }

        /// <summary>头里的字符串被反序列化成 byte[]，要还原成 string</summary>
        private static string? ReadString(IDictionary<string, object?> table, string key)
            => table.TryGetValue(key, out var value) && value is byte[] bytes
                ? Encoding.UTF8.GetString(bytes)
                : null;
    }
}