
using Microsoft.Identity.Client;
using StackExchange.Redis;
using System;
using XuePing.Domain.Common;
using System.Collections.Generic;
using System.Text;
using System.ClientModel.Primitives;
using XuePing.Domain.Abstractions;
using System.Runtime.CompilerServices;

namespace XuePing.Infrastructure.Caching
{
    public class RedisStreamService : IStreamService
    {
        private readonly IDatabase _db;
        public RedisStreamService(IConnectionMultiplexer redis) { _db = redis.GetDatabase(); }
        private static List<StreamMessage> Map(IEnumerable<StreamEntry> entries)
        {
            return entries.Select(e => new StreamMessage
            {
                Id = e.Id.ToString(),
                Field = e.Values.Length > 0 ? e.Values[0].Name.ToString() : string.Empty,
                Value = e.Values.Length > 0 ? e.Values[0].Value.ToString() : string.Empty
            }).ToList();
        }
        private async Task EnsureGroupAsync(string stream, string group, CancellationToken cancellationToken = default)
        {
            try
            {
                await _db.StreamCreateConsumerGroupAsync(stream, group, (RedisValue)"0").WaitAsync(cancellationToken);
            }
            catch (RedisServerException ex)
            when (ex.Message.Contains("BUSYGROUP") || ex.Message.Contains("requires the key to exist"))
            { //组已经存在 或者是stream还没有创建都不需要处理
            }
        }
        public async Task<string> PublishAsync(string stream, string field, string value, int maxLength = 1000, CancellationToken cancellationToken = default)
        {
            var id = await _db.StreamAddAsync(stream, field, value,messageId:null,maxLength:maxLength,useApproximateMaxLength:true).WaitAsync(cancellationToken);
            

            return id.ToString();
        }
        public async Task<List<StreamMessage>> ReadGroupAsync(string stream, string group, string consumer, int count, CancellationToken cancellationToken = default)
        {
           try
            {
             var entries = await _db.StreamReadGroupAsync(stream, group, consumer, (RedisValue)">", count).WaitAsync(cancellationToken);
                return Map(entries);
            }
            catch(RedisServerException ex)when(ex.Message.Contains("NOGROUP"))
            {
                await EnsureGroupAsync(stream, group, cancellationToken);
                try
                {
                    var entries = await _db.StreamReadGroupAsync(stream, group, consumer, (RedisValue)">", count).WaitAsync(cancellationToken);
                    return Map(entries);
                }
                catch(RedisServerException e)when(e.Message.Contains("NOGROUP"))
                {
                    return new List<StreamMessage>();
                }
            }
        }
        public async Task AcknowledgeAsync(string stream, string group, IEnumerable<string> messageIds, CancellationToken cancellationToken = default)
        {
            var ids = messageIds.Select(m => (RedisValue)m).ToArray();
            if (ids.Length > 0)
            {
                await _db.StreamAcknowledgeAsync(stream, group, ids).WaitAsync(cancellationToken);
            }
        }
        public async Task<long> GetPendingCountAsync(string stream, string group, CancellationToken cancellationToken = default)
        {
            var pending = await _db.StreamPendingAsync(stream, group).WaitAsync(cancellationToken);
            return pending.PendingMessageCount;
        }
        public async Task<List<StreamMessage>> ReadPendingAsync(string stream, string group, string consumer, int count, CancellationToken cancellationToken = default)
        {
            try
            {
                var entries = await _db.StreamReadGroupAsync(stream, group, consumer, (RedisValue)"0", count).WaitAsync(cancellationToken);
                return Map(entries);
            }
            catch (RedisServerException ex) when (ex.Message.Contains("NOGROUP"))
            {
                return new List<StreamMessage>();
            }
        }
        public async Task<List<PendingMessage>> GetPendingDetailsAsync(string stream, string group, string consumer, // 入参：流、组、消费者
             int count, CancellationToken cancellationToken = default) // 单次最多查多少条 Pending
        {
            try // 组不存在会抛 NOGROUP
            {
                var infos = await _db.StreamPendingMessagesAsync(stream, group, count, consumer) // XPENDING stream group - + count consumer：只查该消费者名下的 Pending
                    .WaitAsync(cancellationToken); // 支持取消
                return infos.Select(i => new PendingMessage // 逐条映射成 Domain 的 PendingMessage
                {
                    Id = i.MessageId.ToString(), // 消息 ID，如 "1699999999999-0"
                    Consumer = i.ConsumerName.ToString(), // 当前归属的消费者名
                    IdleMs = i.IdleTimeInMilliseconds, // 闲置毫秒数：距离上次投递多久没人处理
                    DeliveryCount = i.DeliveryCount // 已投递次数：超过阈值就进死信，不再重试
                }).ToList(); // 物化为 List 返回
            }
            catch (RedisServerException ex) when (ex.Message.Contains("NOGROUP")) // 组不存在视为“没有 Pending”
            {
                return new List<PendingMessage>(); // 空集合
            }
        }

        // 接管“别人名下”闲置超过 minIdleMs 的消息（消费者宕机恢复）——XAUTOCLAIM 一条命令完成扫描+转移
        public async Task<List<StreamMessage>> ClaimPendingAsync(string stream, string group, string consumer, // 入参：流、组、新消费者
            long minIdleMs, int count, CancellationToken cancellationToken = default) // 只抢闲置超过 minIdleMs 毫秒的消息；单次最多 count 条
        {
            try // 组不存在会抛 NOGROUP
            {
                var result = await _db.StreamAutoClaimAsync(stream, group, consumer, minIdleMs, (RedisValue)"0-0", count) // XAUTOCLAIM stream group consumer minIdle 0-0 COUNT count：从最小 ID 开始扫描并抢占
                    .WaitAsync(cancellationToken); // 支持取消
                return Map(result.ClaimedEntries); // 只取抢到的消息；NextStartId 本轮忽略（量小，每轮从头扫即可）
            }
            catch (RedisServerException ex) when (ex.Message.Contains("NOGROUP")) // 组不存在时没有可抢的消息
            {
                return new List<StreamMessage>(); // 空集合
            }
        }
    }
}
