using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;

using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetCommonFollowsAsync
    {
        Task<Result<List<UserBriefDto>>> ExecuteAsync(long userId, long otherUserId, CancellationToken cancellationToken = default);
    }
}
