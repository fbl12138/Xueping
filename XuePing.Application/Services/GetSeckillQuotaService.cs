using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using Microsoft.EntityFrameworkCore;

using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class GetSeckillQuotaService : BaseService,IGetSeckillQuotaAsync
    {
        private readonly ICacheService _cache;
        
        public GetSeckillQuotaService(IXuePingDbContext dbContext,ICacheService cache) : base(dbContext)
        {
            _cache = cache;
        }
        public async Task<Result<SeckillQuotaDto>> ExecuteAsync(int courseId,CancellationToken cancellationToken = default)
        {
           var quota = await DbContext.SeckillQuotas.FirstOrDefaultAsync(q=>q.CourseId == courseId,cancellationToken);
            if(quota is null) return Result<SeckillQuotaDto>.Fail("课程未开放抢课");
            var stockStr = await _cache.GetStringAsync($"seckill:stock:{courseId}");
            var stock = stockStr is null ? quota.Stock : int.Parse(stockStr);
            var now = DateTime.Now;
            return Result<SeckillQuotaDto>.Ok(new SeckillQuotaDto
            {
                CourseId = courseId,
                Stock = stock,
                BeginTime = quota.BeginTime,
                EndTime = quota.EndTime,
                Started = now >= quota.BeginTime,
                Ended = now >= quota.EndTime
            });

        }
    }
}
