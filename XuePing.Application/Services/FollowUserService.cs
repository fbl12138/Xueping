using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using XuePing.Application.Common;
using XuePing.Domain.Entities;

namespace XuePing.Application.Services
{
    public class FollowUserService : BaseService,IFollowUserAsync
    {
        private readonly IFollowService _follow;
        public FollowUserService(IXuePingDbContext dbContext,IFollowService follow) : base(dbContext)
        {
            _follow = follow;
        }
        public async Task<Result<bool>> ExecuteAsync(long userId,long followUserId,CancellationToken cancellationToken = default)
        {
            if (userId == followUserId) return Result<bool>.Fail("不能关注自己");
            var user = await DbContext.Users.AnyAsync(u => u.Id == followUserId, cancellationToken);
            if (!user) return Result<bool>.Fail("用户不存在");
            var already = await DbContext.Follows.AnyAsync(f => f.UserId == userId && f.FollowUserId == followUserId, cancellationToken);
            if(!already)
            {
                DbContext.Follows.Add(new Follow
                {
                    UserId = userId,
                    FollowUserId = followUserId,
                    CreatedAt = DateTime.Now
                });
                await DbContext.SaveChangesAsync(cancellationToken);
            }
    await _follow.FollowAsync(userId, followUserId, cancellationToken);
           
            return Result<bool>.Ok(true);

        }
    }
}
