using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public interface ISelectCourseAsync
    {
        Task<Result<long>> ExecuteAsync(int courseId,long userId,CancellationToken cancellationToken = default);
    }
}
