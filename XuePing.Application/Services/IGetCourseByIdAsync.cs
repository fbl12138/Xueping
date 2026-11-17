using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetCourseByIdAsync
    {
        Task<Result<CourseDto>> ExecuteAsync(int id,long userId, CancellationToken cancellationToken = default);
    }
}
