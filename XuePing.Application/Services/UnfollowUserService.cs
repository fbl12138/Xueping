using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class UnfollowUserService : BaseService,IUnfollowUserAsync
    {
        private readonly IFollowService _follow;
        public UnfollowUserService(IXuePingDbContext dbContext,IFollowService follow) : base(dbContext)
        {
            _follow = follow;
        }
        public async Task<Result<bool>> ExecuteAsync(long userId,long followUserId,CancellationToken cancellationToken = default)
        {
            if (userId == followUserId) return Result<bool>.Fail("不能取关自己");
            await DbContext.Follows.Where(f => f.UserId == userId && f.FollowUserId == followUserId).ExecuteDeleteAsync(cancellationToken);
            await _follow.UnfollowAsync(userId, followUserId, cancellationToken);
            return Result<bool>.Ok(true);
        }
    }
}
