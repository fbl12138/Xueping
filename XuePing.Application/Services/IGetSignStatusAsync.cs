using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetSignStatusAsync
    {
        Task<Result<SignStatusDto>> ExecuteAsync(long userId, CancellationToken cancellationToken = default);
    }
}
