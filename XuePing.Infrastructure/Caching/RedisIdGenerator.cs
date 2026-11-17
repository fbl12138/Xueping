using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XuePing.Domain.Abstractions;

namespace XuePing.Infrastructure.Caching
{
    public class RedisIdGenerator : IIdGenerator
    {
        private readonly ICacheService _cache;

        private static readonly long BEGIN_TIMESTAMP = new DateTimeOffset(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeSeconds();
        public RedisIdGenerator(ICacheService cache)
        {
            _cache = cache;
        }
        public async Task<long> NextIdAsync(string keyPrefix,CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var key = $"{keyPrefix}:{date}";
            var count = await _cache.IncrAsync(key, cancellationToken);
            if(count== 1)
            {
                await _cache.ExpireAsync(key, TimeSpan.FromDays(2), cancellationToken);
            }
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - BEGIN_TIMESTAMP;
            return (timestamp << 32 ) | count;

        }
    }
}
