using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using XuePing.Application.Common;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Common;

namespace XuePing.Application.Services
{
    public class SeckillOrderConsummerService : BaseService,ISeckillOrderConsumer
    {
        private const string StreamKey = "stream:seckill:order";
        private const string GroupName = "seckill:order:g1";
        private const string DeadStreamKey = "stream:seckill:order:dead";
        private const string Field = "field";
        private const int BatchSize = 10;
        private const int MaxDeliveryCount = 3;
        private const long ClaimIdleMs = 60_000;
        private readonly ISeckillOrderHandler _handler;
        private readonly IStreamService _stream;
        private readonly ILogger<SeckillOrderConsummerService> _logger;
        private static readonly string ConsumerName = $"{Environment.MachineName}-{Guid.NewGuid():N}".Substring(0, 12);
        public SeckillOrderConsummerService
         (
            IXuePingDbContext dbContext,
            IStreamService stream,
            ISeckillOrderHandler hander,
            ILogger<SeckillOrderConsummerService> logger
          ) : base(dbContext)
        {
            _stream = stream;
            _handler = hander;
            _logger = logger;
        }
        private async Task ProcessAsync(List<StreamMessage> messages,CancellationToken cancellationToken = default)
        {
            foreach(var msg in messages)
            {
                try
                {
                    await HandleAsync(msg, cancellationToken);
                    await _stream.AcknowledgeAsync(StreamKey, GroupName, new[] { msg.Id }, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "消费抢课失败msgId = {MsgId}", msg.Id);
                    await TryMoveToDeadLetterAsync(msg, cancellationToken);
                }
            }
        }
        //单挑消息业务处理 反序列化 幂等判断 插入选课记录 提交
        private async Task HandleAsync(StreamMessage msg,CancellationToken cancellationToken = default)
        {
            var order = JsonSerializer.Deserialize<SeckillOrderMessage>(msg.Value);
            if (order is null) return;
            await _handler.HandleAsync(order, cancellationToken);
        }
        //失败消息处理 投递次数达到上限就写死信并ACK 避免卡住Pending头部
        private async Task TryMoveToDeadLetterAsync(StreamMessage msg,CancellationToken cancellationToken = default)
        {
            var details = await _stream.GetPendingDetailsAsync(
                StreamKey, GroupName, ConsumerName, 50, cancellationToken);
            var current = details.FirstOrDefault(d => d.Id == msg.Id);
            if (current is null || current.DeliveryCount < MaxDeliveryCount) return;
            try
            {
                await _stream.PublishAsync(
                    DeadStreamKey, Field, msg.Value, cancellationToken: cancellationToken);
                await _stream.AcknowledgeAsync(StreamKey, GroupName, new[] { msg.Id }, cancellationToken);
                _logger.LogError("消息投递{Count}次仍失败，已经转入死信msgId = {MsgId}", current.DeliveryCount, msg.Id);
                //告诉前端这次失败了 15分钟 TTL 让学生可以重新抢
                try
                {
                    var order = JsonSerializer.Deserialize<SeckillOrderMessage>(msg.Value);
                    if (order is not null)
                    {
                        await _handler.MarkFailedAsync(order, "系统繁忙，请稍后重试", cancellationToken);
                    }
                }
                catch (JsonException)
                {   //消息本身就不是合法 JSON 反序列化不出 userId 无法回写状态 跳过
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "转入死信失败msgId = {msgId}", msg.Id);
            }
        }
        public async Task ConsumeOnceAsync(CancellationToken cancellationToken = default)
        {   //先处理抢来的
            var claimed = await _stream.ClaimPendingAsync(StreamKey, GroupName, ConsumerName, ClaimIdleMs, BatchSize, cancellationToken);
            await ProcessAsync(claimed, cancellationToken);
            //处理自己名下的
            var pending = await _stream.ReadPendingAsync(StreamKey,GroupName,ConsumerName,BatchSize, cancellationToken);
            await ProcessAsync(pending, cancellationToken);
            //读全新的消息
            var newmessage = await _stream.ReadGroupAsync(StreamKey, GroupName, ConsumerName, BatchSize, cancellationToken);
            await ProcessAsync(newmessage, cancellationToken);
        }    
    }
}


