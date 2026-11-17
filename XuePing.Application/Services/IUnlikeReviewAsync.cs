using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public interface IUnlikeReviewAsync
    {
        Task<Result<bool>> ExecuteAsync(long reviewId, long userId, CancellationToken cancellationToken = default);
    }
}
