using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace XuePing.Application.Services
{
    public class CreateReviewService : BaseService,ICreateReviewAsync
    {
        public CreateReviewService(IXuePingDbContext dbContext) : base(dbContext) { }
        public async Task<Result<ReviewDto>> ExecuteAsync(CreateReviewRequest request,long userId,CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            var review = new Review
            {   UserId = userId,
                CourseId = request.CourseId,
                Content = request.Content,
                Score = request.Score,
                LikeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            };
            await DbContext.Reviews.AddAsync(review);
            await DbContext.SaveChangesAsync(cancellationToken);
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            return Result<ReviewDto>.Ok(new ReviewDto
            {
                Id = review.Id,
                UserId = userId,
                NickName = user?.NickName ?? string.Empty,
                CourseId = review.CourseId,
                Content = review.Content,
                Score = review.Score,
                LikeCount = review.LikeCount,
                Liked = false,
                CreatedAt = review.CreatedAt
            });

        }
    }
}
