using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface ISendCodeAsync
    {
        Task<Result<string>> ExecuteAsync(SendCodeRequest request, CancellationToken cancellationToken = default);
    }
}
