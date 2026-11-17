using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetReviewByCourseAsync
    {
        Task<Result<List<ReviewDto>>> ExecuteAsync(int courseId,long currentUserId,CancellationToken cancellationToken = default);
    }
}
