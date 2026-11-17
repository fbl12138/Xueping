using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IUpdateCourseAsync
    {
        Task<Result<CourseDto>> ExecuteAsync(int id, UpdateCourseRequest request, CancellationToken cancellationToken = default);
    }
}
