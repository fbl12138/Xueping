
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class GetReviewLikeRankService : BaseService,IGetReviewLikeRankAsync
    {
        private readonly IReviewLikeService _like;
        public GetReviewLikeRankService(IXuePingDbContext dbContext,IReviewLikeService like) : base(dbContext)
        { _like = like;  }
        public async Task<Result<List<UserBriefDto>>> ExecuteAsync(long reviewId,int count,CancellationToken cancellationToken = default)
        {
            var ids = await _like.GetTopLikedUserIdsAsync(reviewId, count, cancellationToken);
            if (ids.Count == 0) return Result<List<UserBriefDto>>.Ok(new List<UserBriefDto>());
            
            var users = await DbContext.Users.Where(u => ids.Contains(u.Id)).Select(u => new UserBriefDto
            {
                Id = u.Id,
                NickName = u.NickName,
                Icon = u.Icon
            }).ToListAsync(cancellationToken);
            var map = users.ToDictionary(u => u.Id);
            var ordered = ids.Where(map.ContainsKey).Select(id => map[id]).ToList();
            return Result<List<UserBriefDto>>.Ok(ordered);

        }
    }
}
