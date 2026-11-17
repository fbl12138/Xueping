using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public interface IDeleteCourseAsync
    {
        Task<Result<bool>> ExecuteAsync(int id, CancellationToken cancellationToken = default);
    }
}
