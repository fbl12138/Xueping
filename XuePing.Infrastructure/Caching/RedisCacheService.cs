using StackExchange.Redis;
using System.Text.Json;
using XuePing.Domain.Abstractions;

namespace XuePing.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        //取缓存对象
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var value = await _db.StringGetAsync(key).WaitAsync(cancellationToken);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value.ToString())!;
        }
        //存缓存对象
        public async Task SetAsync<T>(string key, T value,TimeSpan? expiry = null , CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            var expiration = expiry.HasValue ? (Expiration)expiry.Value : Expiration.Default;
            await _db.StringSetAsync(key, json, expiration).WaitAsync(cancellationToken);
        }
        //取字符串 验证码/登录态使用 不需要序列化
        public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            var value = await _db.StringGetAsync(key).WaitAsync(cancellationToken);
            return value.IsNullOrEmpty ? null : value.ToString();
        }
        //存字符串
        public async Task SetStringAsync(string key,string value,TimeSpan? expiry = null,CancellationToken cancellationToken = default)
        {
            var expiration = expiry.HasValue ? expiry.Value : Expiration.Default;
             await _db.StringSetAsync(key, value, expiration).WaitAsync(cancellationToken);
        }
            //删除
        public async Task RemoveAsync(string key,CancellationToken cancellationTokne = default)
        {
            await _db.KeyDeleteAsync(key).WaitAsync(cancellationTokne);

        }

        //不存在就添加
        public async Task<bool> SetIfNotExistsAsync(string key,string value,TimeSpan expiry,CancellationToken cancellationToken = default)
        {
            return await _db.StringSetAsync(key, value, expiry, When.NotExists).WaitAsync(cancellationToken);
           
        }
        //自增
        public async Task<long> IncrAsync(string key,CancellationToken cancellationToken = default)
        {
            return await _db.StringIncrementAsync(key).WaitAsync(cancellationToken);
        }
        //过期时间
        public async Task<bool> ExpireAsync(string key,TimeSpan expiry,CancellationToken cancellationToken = default)
        { 
                return await _db.KeyExpireAsync(key,expiry).WaitAsync(cancellationToken);
           
        }
    }
   
}
