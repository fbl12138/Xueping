using Microsoft.IdentityModel.Tokens.Experimental;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Text;
using XuePing.Domain.Abstractions;

namespace XuePing.Infrastructure.Caching
{
    public class RedisSignService : ISignService
    {
        private readonly IDatabase _db;
        public RedisSignService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        private const int MonthBitWidth = 31;
        private static string SignKey(long userId,DateTime month ) => $"sign:{userId}:{month:yyyyMM}";
        public async Task<bool> SignAsync(long userId,CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            var key = SignKey(userId, now);
            long offset = now.Day - 1;
            bool alreadySigned = await _db.StringSetBitAsync(key, offset, true).WaitAsync(cancellationToken);
            if (alreadySigned) return false;
            var expireAt = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(1);
            await _db.KeyExpireAsync(key, expireAt - now).WaitAsync(cancellationToken);
            return true;
        }
        public async Task<List<int>> GetSignedDaysAsync(long userId,CancellationToken cancellationToken =default)
        {
            var now = DateTime.Now;
            long bits = await GetMonthBitsAsync(SignKey(userId,now), MonthBitWidth, cancellationToken);
            var days = new List<int>(MonthBitWidth); 
            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            for(int day= 1;day<=daysInMonth;day++)
            {
                int shift = MonthBitWidth - day;
                if (((bits >> shift) & 1L) == 1L) days.Add(day); 
            }
            return days;
        }
        public async Task<int> GetConsecutiveDaysAsync(long userId,CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            int today = now.Day;
            long bits = await GetMonthBitsAsync(SignKey(userId, now), today, cancellationToken);
            ulong mask = (ulong)bits & ((1UL << today) - 1UL);
            if ((mask & 1UL) == 0UL) return 0;
            return BitOperations.TrailingZeroCount(~mask);
        }
        private async Task<long> GetMonthBitsAsync(string key,int width,CancellationToken cancellationToken = default)
        {
            var raw = await _db.ExecuteAsync("BITFIELD", key, "GET", $"u{width}", 0).WaitAsync(cancellationToken);
            return (long)((RedisResult[])raw!)[0];
        }
    }
}
