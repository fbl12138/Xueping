using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class UnlikeReviewService : BaseService,IUnlikeReviewAsync
    {
        private readonly IReviewLikeService _like;
        public UnlikeReviewService(IXuePingDbContext dbContext,IReviewLikeService like): base(dbContext)
        {
            _like = like;
        }
        public async Task<Result<bool>> ExecuteAsync(long reviewId,long userId,CancellationToken cancellationToken = default)
        {
        if(!await _like.UnlikeAsync(reviewId,userId,cancellationToken)) return Result<bool>.Fail("尚未点赞");
            await DbContext.Reviews.Where(p => p.Id == reviewId && p.LikeCount > 0).ExecuteUpdateAsync(s => s.SetProperty(r => r.LikeCount, r => r.LikeCount - 1), cancellationToken);
            return Result<bool>.Ok(true);

        }
    }
}
