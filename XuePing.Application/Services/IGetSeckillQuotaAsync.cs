using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Entities;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetSeckillQuotaAsync
    {
        Task<Result<SeckillQuotaDto>> ExecuteAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
