using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IGetCourseUvAsync
    {
        Task<Result<CourseUvDto>> ExecuteAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
