using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetReviewLikeRankAsync
    {
        Task<Result<List<UserBriefDto>>> ExecuteAsync(long reviewId, int count, CancellationToken cancellationToken = default);
    }
}
