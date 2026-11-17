using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.Services
{
    public interface ISeckillOrderConsumer
    {
        Task ConsumeOnceAsync(CancellationToken cancellationToken = default);
    }
}
