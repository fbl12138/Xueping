using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public interface ISignAsync
    {
        Task<Result<bool>> ExecuteAsync(long userId, CancellationToken cancellationToken = default);
    }
}
