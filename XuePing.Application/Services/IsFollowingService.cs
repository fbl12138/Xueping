using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class IsFollowingService : BaseService,IIsFollowingAsync
    {
        private readonly IFollowService _follow;
        public IsFollowingService(IXuePingDbContext dbContext,IFollowService follow) : base(dbContext)
        {
            _follow = follow;
        }
        public async Task<Result<bool>> ExecuteAsync(long userId,long followUserId,CancellationToken cancellationToken = default)
        {
            var result = await _follow.IsFollowingAsync(userId, followUserId, cancellationToken);
            return Result<bool>.Ok(result);
        }
    }
}
