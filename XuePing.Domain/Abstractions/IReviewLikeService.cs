using System;
using System.Collections.Generic;
using System.Text;
namespace XuePing.Domain.Abstractions
{
    public interface IReviewLikeService
    {
        //点赞
        Task<bool> LikeAsync(long reviewId, long userId, CancellationToken cancellationToken = default);
        //取消赞
        Task<bool> UnlikeAsync(long reviewId, long userId, CancellationToken cancellationToken = default);
        //是否已赞
        Task<bool> IsLikedAsync(long reviewId, long userId, CancellationToken cancellationToken = default);
        //点赞数
        Task<long> GetLikeCountAsync(long reviewId, CancellationToken cancellationToken = default);
        //批量已赞状态(分页)
        Task<Dictionary<long,bool>> GetLikeStatusAsync(IEnumerable<long> reviewIds, long userId, CancellationToken cancellationToken = default);
        //最近点赞的Top5人
        Task<List<long>> GetTopLikedUserIdsAsync(long reviewId, int count, CancellationToken cancellationToken = default);
    }
}
