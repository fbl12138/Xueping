using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface ICreateReviewAsync
    {
        Task<Result<ReviewDto>> ExecuteAsync(CreateReviewRequest request,long userId,CancellationToken cancellationToken = default);
    }
}
