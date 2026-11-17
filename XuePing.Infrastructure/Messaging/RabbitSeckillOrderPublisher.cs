using System.Text.Json;                          // 序列化消息体
using System.Threading;
using RabbitMQ.Client;                           // IChannel / BasicProperties / CreateChannelOptions
using XuePing.Domain.Abstractions;               // ISeckillOrderPublisher
using XuePing.Domain.Common;                     // SeckillOrderMessage

namespace XuePing.Infrastructure.Messaging
{
    //发布者Confirm+持久化消息
    public sealed class RabbitSeckillOrderPublisher : ISeckillOrderPublisher,IAsyncDisposable
    {
        private readonly RabbitMqConnectionProvider _provider;//复用单列连接
        private readonly SemaphoreSlim _gate = new(1, 1);
        private IChannel? _channel;

        private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
        {
            if (_channel is { IsOpen: true }) return _channel;
            var connection = await _provider.GetAsync(cancellationToken);
            _channel =await connection.CreateChannelAsync(
                new CreateChannelOptions(
                    publisherConfirmationsEnabled: true, //开启Confirm机制
                    publisherConfirmationTrackingEnabled: true), //让BasicPublishAsync真正等待确认
                cancellationToken: cancellationToken);
            await RabbitOrderTopology.DeclareAsync(_channel, cancellationToken);
            return _channel;
        }
        public async ValueTask DisposeAsync()
        {
            if(_channel is not null) await _channel.DisposeAsync();
            _gate.Release();
        }
        
        public RabbitSeckillOrderPublisher(RabbitMqConnectionProvider provider)
        {
            _provider = provider;
        }
        public async Task PublishAsync(SeckillOrderMessage message,CancellationToken cancellationToken = default)
        {
            var channel = await GetChannelAsync(cancellationToken);
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var props = new BasicProperties
            {
                Persistent = true, //消息持久化
                ContentType = "application/json", //消息是json类型
            };
            await _gate.WaitAsync(cancellationToken);
            try
            {
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: RabbitOrderTopology.QueueName,
                    mandatory: true,//消息投递失败不会丢弃 会退还给生产者
                    basicProperties: props,
                    body: body,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                _gate.Release();
            }
        }
        
    }
}
