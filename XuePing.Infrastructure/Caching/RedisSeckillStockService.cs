
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Common;

namespace XuePing.Infrastructure.Caching
{
    public class RedisSeckillStockService : ISeckillStockService
    {
        private readonly IDatabase _db;
        private const string DeductScript = @"local stock = redis.call('get', KEYS[1])
                                              if (not stock or tonumber(stock) <= 0) then return 0 end
                                              if (redis.call('sismember', KEYS[2], ARGV[1]) == 1) then return 2 end
                                                redis.call('incrby', KEYS[1], -1)
                                                redis.call('sadd', KEYS[2], ARGV[1])
                                                return 1";
        public RedisSeckillStockService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        public async Task<SeckillResult> TryDeductAsync(int courseId, long userId, CancellationToken cancellationToken = default)
        {
            var stockKey = $"seckill:stock:{courseId}";
            var userKey = $"seckill:users:{courseId}";
            var keys = new RedisKey[]
            {
                stockKey,
                userKey
            };
            var values = new RedisValue[]
            {
                userId.ToString()
            };
            var result = await _db.ScriptEvaluateAsync(DeductScript, keys, values).WaitAsync(cancellationToken);
            var code = (long)result;
            return code switch
            {
                0 => SeckillResult.SoldOut,
                1 => SeckillResult.Success,
                2 => SeckillResult.Repeat,
                _ => throw new InvalidOperationException($"未知返回码:{code}")
            };

        }
    }
}
