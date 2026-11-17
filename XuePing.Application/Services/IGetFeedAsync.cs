using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.DTOs;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public interface IGetFeedAsync
    {
        Task<Result<FeedPageDto>> ExecuteAsync(long userId,long? lastId,int offset, int count, CancellationToken cancellationToken = default);
    }
}
