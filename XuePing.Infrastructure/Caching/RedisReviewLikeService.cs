using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Entities;
using System.Security.Claims;
using System.Reflection.Metadata.Ecma335;
using XuePing.Application.Common;

namespace XuePing.Infrastructure.Caching
{
    public class RedisReviewLikeService : IReviewLikeService
    {
        private readonly IDatabase _db;
        public RedisReviewLikeService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        private static string Key(long reviewId)
        { return $"review:liked:{reviewId}"; }
        //点赞
        public async Task<bool> LikeAsync(long reviewId,long userId,CancellationToken cancellationToken = default)
        {
            var key =Key(reviewId);
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return await _db.SortedSetAddAsync(key, userId, time).WaitAsync(cancellationToken);
       
        }
        //取消点赞
        public async Task<bool> UnlikeAsync(long reviewId,long userId,CancellationToken cancellationToken = default)
        {
            var key = Key(reviewId);
            return await _db.SortedSetRemoveAsync(key,userId).WaitAsync(cancellationToken);
        }
        //是否已经点赞
        public async Task<bool> IsLikedAsync(long reviewId,long userId,CancellationToken cancellationToken = default)
        {
            var key = Key(reviewId);
            
        var result = await _db.SortedSetScoreAsync(key, userId).WaitAsync(cancellationToken);
          
            return result.HasValue;
        }
        //获得点赞数量
        public async Task<long> GetLikeCountAsync(long reviewId,CancellationToken cancellationToken = default)
        {
            var key = Key(reviewId);
            return await _db.SortedSetLengthAsync(key).WaitAsync(cancellationToken);
            
                
        }
        //点赞情况分页
        public async Task<Dictionary<long,bool>> GetLikeStatusAsync(IEnumerable<long> reviewIds,long userId,CancellationToken cancellationToken =default)
        {   var ids = reviewIds.ToList();
            var result = new Dictionary<long, bool>(ids.Count);
            if(ids.Count == 0 ) return result;
            var batch = _db.CreateBatch();
            var tasks = new Task<double?>[ids.Count];
            for(var i = 0;i<ids.Count;i++)
            {
                tasks[i] = batch.SortedSetScoreAsync(Key(ids[i]), userId);
            }
            batch.Execute();
            var scores = await Task.WhenAll(tasks).WaitAsync(cancellationToken);
            for(var i = 0;i<ids.Count;i++)
            {
                result[ids[i]] = scores[i].HasValue;
            }
            return result;
        }
        //取最近点赞时间
        public async Task<List<long>> GetTopLikedUserIdsAsync(long reviewId,int count,CancellationToken cancellationToken = default)
        {
            var key = Key(reviewId);
            var top5 = await _db.SortedSetRangeByRankAsync(key, 0, count - 1, Order.Descending).WaitAsync(cancellationToken);
            return top5.Select(p => (long)p).ToList();
        }   
        

        
    }
}
