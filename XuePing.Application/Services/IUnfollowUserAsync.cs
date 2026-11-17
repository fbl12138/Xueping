using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public interface IUnfollowUserAsync
    {
        Task<Result<bool>> ExecuteAsync(long userId, long followUserId, CancellationToken cancellationToken = default);
    }
}
