using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Common;

namespace XuePing.Application.Services
{
    public class GetSeckillStatusService : BaseService,IGetSeckillStatusAsync
    {
        private const string PendingValue = "Pending";
        private const string FailedPrefix = "FAILED:";
        private readonly ICacheService _cache;
        public GetSeckillStatusService(IXuePingDbContext dbContext, ICacheService cache) : base(dbContext)
        {
            _cache = cache;
        }
        public async Task<Result<SeckillStatusDto>> ExecuteAsync(int courseId,long userId,CancellationToken cancellationToken = default)
        {
            var selection = await DbContext.CourseSelections.FirstOrDefaultAsync(d=>d.CourseId == courseId && d.UserId == userId).WaitAsync(cancellationToken);
            if(selection is not null)
            {
                return Result<SeckillStatusDto>.Ok(new SeckillStatusDto
                {
                    CourseId = courseId,
                    Status = "Success",
                    OrderId = selection.Id
                });
            }
            var key = SeckillStatusKey.For(courseId, userId);
            var value = await _cache.GetStringAsync(key, cancellationToken);
            if(value is null)
            {
                return Result<SeckillStatusDto>.Ok(new SeckillStatusDto { CourseId = courseId, Status = "None" });

            }
            if(value == PendingValue)
            {
                return Result<SeckillStatusDto>.Ok(new SeckillStatusDto { CourseId = courseId, Status = "Pending" });

            }
            if(value.StartsWith(FailedPrefix))
            {
                return Result<SeckillStatusDto>.Ok(new SeckillStatusDto
                {
                    CourseId = courseId,
                    Status = "Failed",
                    Message = value[FailedPrefix.Length..]
                });
            }
            return Result<SeckillStatusDto>.Ok(new SeckillStatusDto { CourseId = courseId, Status = "None" });
        }
    }
}
