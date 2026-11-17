using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Abstractions
{
    public interface IIdGenerator
    {
        Task<long> NextIdAsync(string keyPrefix, CancellationToken cancellationToken = default);
    }
}
