using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Abstractions
{
    public interface ICacheService
    {   //泛指：取存对象，后续接口服务负责JSON序列化和反序列化  
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

        //字符串：验证码、登录态这类简单值，不折腾序列化
        Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
        Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

        //删除缓存
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        //不存在才能添加SetNx
        Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan expiry, CancellationToken cancellationToken = default);
        //自增
        Task<long> IncrAsync(string key, CancellationToken cancellationToken = default);
        //过期时间
        Task<bool> ExpireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default);
    }
}
