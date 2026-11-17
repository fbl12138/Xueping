using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetSeckillStatusAsync
    {
        Task<Result<SeckillStatusDto>> ExecuteAsync(int courseId, long userId, CancellationToken cancellationToken = default);
    }
}
