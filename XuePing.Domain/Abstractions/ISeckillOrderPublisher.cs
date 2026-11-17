using XuePing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Abstractions
{
    public interface ISeckillOrderPublisher
    {
        Task PublishAsync(SeckillOrderMessage message, CancellationToken cancellationToken = default);
    }
}
