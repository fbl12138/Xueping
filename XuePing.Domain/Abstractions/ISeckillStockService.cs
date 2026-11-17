using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Common;

namespace XuePing.Domain.Abstractions
{
    public interface ISeckillStockService
    {
        Task<SeckillResult> TryDeductAsync(int courseId, long userId, CancellationToken cancellationToken = default);
    }
}
