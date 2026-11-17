using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using Microsoft.EntityFrameworkCore;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Common;
using XuePing.Domain.Entities;
//两个消息队列 消费者处理消息时 成功调用handle失败用markfail
namespace XuePing.Application.Services
{
    public class SeckillOrderHandler : BaseService,ISeckillOrderHandler
    {
        private ICacheService _cache;
        private IIdGenerator _idGenerator;
        public SeckillOrderHandler(IXuePingDbContext dbContext,ICacheService cache, IIdGenerator idGenerator):base (dbContext)
        {
            _cache = cache;
            _idGenerator = idGenerator;
        }
        public async Task HandleAsync(SeckillOrderMessage order,CancellationToken cancellationToken = default)
        {
            var exists = await DbContext.CourseSelections.FirstOrDefaultAsync(s => s.CourseId == order.CourseId
            && s.UserId == order.UserId, cancellationToken);
            if(exists is not null)
            {
                await _cache.RemoveAsync(SeckillStatusKey.For(order.CourseId, order.UserId), cancellationToken);
                return;
            }
            var orderId = await _idGenerator.NextIdAsync("seckill:order", cancellationToken);
            DbContext.CourseSelections.Add(new CourseSelection
            {
                Id = orderId,
                UserId = order.UserId,
                CourseId = order.CourseId,
                Status = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
            await DbContext.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(SeckillStatusKey.For(order.CourseId, order.UserId), cancellationToken);
        }
        public async Task MarkFailedAsync(SeckillOrderMessage order, string reason, CancellationToken cancellationToken = default)
        {
            await _cache.SetStringAsync(
                SeckillStatusKey.For(order.CourseId, order.UserId),
                $"FAILED:{reason}",
                TimeSpan.FromMinutes(15),
                cancellationToken);
        }
    }
}
