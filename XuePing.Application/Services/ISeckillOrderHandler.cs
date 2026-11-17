using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Common;

namespace XuePing.Application.Services
{
    public interface ISeckillOrderHandler
    {
        //幂等入库 已存在则查状态 不重复插
        Task HandleAsync(SeckillOrderMessage order, CancellationToken cancellationToken = default);
        //失败落状态
        Task MarkFailedAsync(SeckillOrderMessage order, string reason, CancellationToken cancellationToken = default);
    }
}
