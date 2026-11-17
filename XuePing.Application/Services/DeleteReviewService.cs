using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace XuePing.Application.Services
{
    public class DeleteReviewService : BaseService,IDeleteReviewAsync
    {

        public DeleteReviewService(IXuePingDbContext dbContext) : base(dbContext) { }
        public async Task<Result<bool>> ExecuteAsync(long reviewId,long userId,CancellationToken cancellationToken = default )
        {
            var review = await DbContext.Reviews.FirstOrDefaultAsync(p => p.Id == reviewId && p.UserId == userId, cancellationToken);
            if (review is null) return Result<bool>.Fail("评论不存在或者无权删除");
            DbContext.Reviews.Remove(review);
            await DbContext.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true);
        }
    }
}
