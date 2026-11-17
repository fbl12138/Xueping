using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace XuePing.Application.Services
{
    public class LikeReviewService : BaseService,ILikeReviewAsync
    {
        private readonly IReviewLikeService _like;
        public LikeReviewService(IXuePingDbContext dbContext, IReviewLikeService like) : base(dbContext)
        { _like = like; }
        public async Task<Result<bool>> ExecuteAsync(long reviewId,long userId,CancellationToken cancellationToken = default)
        {
            var review = await DbContext.Reviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
            if (review is null)
                return Result<bool>.Fail("评论不存在");
           var result =  await _like.LikeAsync(reviewId, userId, cancellationToken);
            if (!result) return Result<bool>.Fail("请勿重复点赞");
            await DbContext.Reviews.Where(r => r.Id == reviewId).ExecuteUpdateAsync(s => s.SetProperty(r => r.LikeCount,r=>r.LikeCount + 1), cancellationToken);
            return Result<bool>.Ok(result);
        }
    }
}
