 using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface ILoginAsync
    {
        Task<Result<LoginResponseDto>> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
