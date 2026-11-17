using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class GetCourseUvService : BaseService,IGetCourseUvAsync
    {
        private readonly ICourseUvService _uv;
        public GetCourseUvService(IXuePingDbContext dbContext, ICourseUvService uv) : base(dbContext) { _uv = uv; }
        public async Task<Result<CourseUvDto>> ExecuteAsync(int courseId,CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            var Today = await _uv.GetDailyUvAsync(courseId, now, cancellationToken);
            var Total = await _uv.GetTotalUvAsync(courseId, cancellationToken);
            return Result<CourseUvDto>.Ok(new CourseUvDto
            {
                TodayUv = Today,
                TotalUv = Total
                
            });

        }
    }
}
