using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Abstractions;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public class GetReviewsByCourseService : BaseService,IGetReviewByCourseAsync
    {
        private readonly IReviewLikeService _like;
        public GetReviewsByCourseService(IXuePingDbContext dbContext,IReviewLikeService like) : base(dbContext)
        {
            _like = like;
        }
        public async Task<Result<List<ReviewDto>>> ExecuteAsync(int courseId,long currentUserId,CancellationToken cancellationToken = default)
        {  
            var list = await (from r in DbContext.Reviews
                              join u in DbContext.Users
                              on r.UserId equals u.Id
                              where r.CourseId == courseId
                              orderby r.CreatedAt descending
                              select new ReviewDto
                              {
                                  Id = r.Id,
                                  UserId = r.UserId,
                                  NickName = u.NickName,
                                  CourseId = r.CourseId,
                                  Content = r.Content,
                                  Score = r.Score,
                                  LikeCount = r.LikeCount,
                                  Liked = false ,      // Step 2 换成 Redis 查出来的真实值
                                  CreatedAt = r.CreatedAt
                              }).ToListAsync(cancellationToken);
            if(list.Count == 0) return Result<List<ReviewDto>>.Ok(list);
            var likeStatus = await _like.GetLikeStatusAsync(
                list.Select(p=>p.Id).ToList(),currentUserId,cancellationToken);
            foreach(var dto in list)
            {
                dto.Liked = likeStatus.TryGetValue(dto.Id, out var liked) && liked;
            }
            return Result<List<ReviewDto>>.Ok(list);
        }
    }
}
