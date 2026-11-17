using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using XuePing.Domain.Abstractions;

namespace XuePing.Infrastructure.Caching
{
    public class RedisCourseUvService : ICourseUvService
    {
        private readonly IDatabase _db;
        public RedisCourseUvService(IConnectionMultiplexer redis) { _db = redis.GetDatabase(); }
        private static string GetDailyKey(int courseId, DateTime date) => $"uv:course:{courseId}:{date:yyyyMMdd}";
        private static string GetTotalKey(int courseId) => $"uv:course:{courseId}:total";
        //标记
        public async Task RecordAsync(int courseId,long userId,CancellationToken cancellationToken = default)
        { var now = DateTime.Now;
            var dailyKey = GetDailyKey(courseId, DateTime.Now.Date);
            var totalKey = GetTotalKey(courseId);
            var ttl = new DateTime(now.Year, now.Month, now.Day).AddDays(2) - now;
            var batch = _db.CreateBatch();
            var tasks = new List<Task>();
            tasks.Add(batch.HyperLogLogAddAsync(dailyKey,userId));
            tasks.Add(batch.HyperLogLogAddAsync(totalKey,userId));
            tasks.Add(batch.KeyExpireAsync(dailyKey,ttl));
            batch.Execute();
            await Task.WhenAll(tasks).WaitAsync(cancellationToken);
        }
        //获取某天的访客量
       public async Task<long> GetDailyUvAsync(int courseId,DateTime date,CancellationToken cancellationToken = default)
        {
            var dailyKey = GetDailyKey(courseId, date);
            var count = await _db.HyperLogLogLengthAsync(dailyKey).WaitAsync(cancellationToken);
            return count;
        }
        //获取指定课程总访客量
        public async Task<long> GetTotalUvAsync(int courseId,CancellationToken cancellationToken = default)
        {
            var totalKey = GetTotalKey(courseId);
            return await _db.HyperLogLogLengthAsync(totalKey).WaitAsync(cancellationToken);
        }

    }
}
