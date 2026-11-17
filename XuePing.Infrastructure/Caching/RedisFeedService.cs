
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Common;

namespace XuePing.Infrastructure.Caching
{
    public class RedisFeedService : IFeedService
    {
        private readonly IDatabase _db;
        public RedisFeedService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        private static string FeedKey(long userId) => $"feed:{userId}";
        public async Task PushAsync(IReadOnlyCollection<long> receiverIds,long postId,long scoreMs,CancellationToken cancellationToken = default)
        {
            if (receiverIds.Count == 0) return;
            var batch = _db.CreateBatch();
            var tasks = new List<Task>(receiverIds.Count * 2);
            foreach(var uid in receiverIds)
            {
                // 先写入，再同批次裁剪：rank 0 是最旧的（score 为时间戳升序），保留最新 1000 条
                tasks.Add(batch.SortedSetAddAsync(FeedKey(uid), postId, scoreMs));
                tasks.Add(batch.SortedSetRemoveRangeByRankAsync(FeedKey(uid), 0, -1001));
            }
            batch.Execute();
            await Task.WhenAll(tasks).WaitAsync(cancellationToken);
        }
        public async Task<List<FeedEntry>> GetFeedIdsAsync(long userId,long? lastId,int offset,int count,CancellationToken cancellationToken = default)
        {
            double max = lastId.HasValue ? lastId.Value : double.PositiveInfinity;
            var entries = await _db.SortedSetRangeByScoreWithScoresAsync(
                FeedKey(userId),
                start:0,
                stop:max,
                exclude:Exclude.None,
                order:Order.Descending,
                skip:offset,
                take:count)
                .WaitAsync(cancellationToken);
            return entries.Select(e=>new FeedEntry
            {
                PostId = (long)e.Element,
                Score = (long)e.Score
            }).ToList();
        }
    }
}
