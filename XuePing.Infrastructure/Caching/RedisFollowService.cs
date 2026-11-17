
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32.SafeHandles;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Abstractions;

namespace XuePing.Infrastructure.Caching
{
    public class RedisFollowService : IFollowService
    {
        private readonly IDatabase _db;
        public RedisFollowService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        private static string FollowKey(long userId) => $"follow:user:{userId}";
        private static string FansKey(long followUserId) => $"fans:user:{followUserId}";
        //点关注
        public async Task<bool> FollowAsync(long userId,long followUserId,CancellationToken cancellationToken = default)
        {
            var key = FollowKey(userId);
            var bekey = FansKey(followUserId);
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var batch = _db.CreateBatch();
            var addFollow = batch.SetAddAsync(key, followUserId);
            var addFans =batch.SetAddAsync(bekey, userId);
            batch.Execute();
            var ok = await addFollow.WaitAsync(cancellationToken);
            await addFans.WaitAsync(cancellationToken);
            return ok;
        }
        //取消关注
        public async Task<bool> UnfollowAsync(long userId,long followUserId,CancellationToken cancellationToken = default)
        {
            var batch = _db.CreateBatch();
            var delFollow = batch.SetRemoveAsync(FollowKey(userId), followUserId);
            var delFans = batch.SetRemoveAsync(FansKey(followUserId), userId);
            batch.Execute();
            var ok = await delFollow.WaitAsync(cancellationToken);
            await delFans.WaitAsync(cancellationToken);
            return ok;
        }
        //是否关注
        public async Task<bool> IsFollowingAsync(long userId,long followUserId,CancellationToken cancellationToken = default)
        {
            return await _db.SetContainsAsync(FollowKey(userId), followUserId).WaitAsync(cancellationToken);
        }
        //我的关注
        public async Task<List<long>> GetFollowIdsAsync(long userId,CancellationToken cancellationToken = default)
        {
            var members = await _db.SetMembersAsync(FollowKey(userId)).WaitAsync(cancellationToken);
            return members.Select(p => (long)p).ToList();
        }
        //我的粉丝
        public async Task<List<long>> GetFansIdsAsync(long userId,CancellationToken cancellationToken = default)
        {
            var memebers = await _db.SetMembersAsync(FansKey(userId)).WaitAsync(cancellationToken);
            return memebers.Select(p => (long)p).ToList();
        }
        //获取共同粉丝
        public async Task<List<long>> GetCommonFollowIdsAsync(long userId,long otheruserId,CancellationToken cancellation = default)
        {
            var common = await _db.SetCombineAsync(SetOperation.Intersect, FollowKey(userId), FollowKey(otheruserId)).WaitAsync(cancellation);
            return common.Select(p => (long)p).ToList();
        }

    }
}
