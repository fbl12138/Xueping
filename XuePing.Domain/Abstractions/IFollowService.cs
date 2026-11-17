using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Abstractions
{
    public interface IFollowService
    {
        Task<bool> FollowAsync(long userId, long followUserId, CancellationToken cancellationToken = default);
        Task<bool> UnfollowAsync(long userId, long followUserId, CancellationToken cancellationToken = default);
        Task<bool> IsFollowingAsync(long userId, long followUserId, CancellationToken cancellationToken = default);
        Task<List<long>> GetFollowIdsAsync(long userId, CancellationToken cancellationToken = default);   // 我的关注列表
        Task<List<long>> GetFansIdsAsync(long userId, CancellationToken cancellationToken = default);     // 我的粉丝列表
        Task<List<long>> GetCommonFollowIdsAsync(long userId, long otherUserId, CancellationToken cancellationToken = default); // 共同关注
    }
}
