using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetAllCoursesAsync
    {
        Task<Result<List<CourseDto>>> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
