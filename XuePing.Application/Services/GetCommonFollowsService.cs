using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace XuePing.Application.Services
{
    public class GetCommonFollowsService : BaseService,IGetCommonFollowsAsync
    {
        private readonly IFollowService _follow;
        public GetCommonFollowsService(IXuePingDbContext dbContext,IFollowService follow) : base(dbContext)
        {
            _follow = follow;
        }
        public async Task<Result<List<UserBriefDto>>> ExecuteAsync(long userId,long otherUserId,CancellationToken cancellationToken = default)
        {
            var ids = await _follow.GetCommonFollowIdsAsync(userId, otherUserId, cancellationToken);
            if (ids.Count == 0) return Result<List<UserBriefDto>>.Ok(new List<UserBriefDto>());
           var list = await DbContext.Users.Where(u => ids.Contains(u.Id)).Select(u => new UserBriefDto
            {
                Id = u.Id,
                NickName = u.NickName,
                Icon = u.Icon
            }).ToListAsync(cancellationToken);
            return Result<List<UserBriefDto>>.Ok(list);
        }
    }
}
