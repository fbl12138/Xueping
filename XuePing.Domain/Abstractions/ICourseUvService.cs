using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Abstractions
{
    public interface ICourseUvService
    {
        Task RecordAsync(int courseId, long userId, CancellationToken cancellationToken = default);
        Task<long> GetDailyUvAsync(int courseId, DateTime date, CancellationToken cancellationToken = default);
        Task<long> GetTotalUvAsync(int courseId, CancellationToken cancellationToken = default);
    }
}
