using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using XuePing.Domain.Entities;
using XuePing.Domain.Common;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace XuePing.Application.Services
{
    public class SelectCourseService : BaseService,ISelectCourseAsync
    {
        private readonly IIdGenerator _idGenerator;
        private readonly ISeckillStockService _seckill;
        private readonly ICacheService _cache;
        private readonly ISeckillOrderPublisher _publisher;
        public SelectCourseService(IXuePingDbContext DbContext,IIdGenerator idGenerator,ISeckillStockService seckill,ICacheService cache,ISeckillOrderPublisher publisher) : base(DbContext)
        {
            _idGenerator = idGenerator;
            _seckill = seckill;

            _cache = cache;
           _publisher = publisher;
        }
        public async Task<Result<long>> ExecuteAsync(int courseId, long userId, CancellationToken cancellationToken = default)
        {   //时间窗口校验
            var quota = await DbContext.SeckillQuotas.FirstOrDefaultAsync(q => q.CourseId == courseId, cancellationToken);
            if (quota is null) return Result<long>.Fail("课程未开放抢课");
            if (DateTime.Now < quota.BeginTime) return Result<long>.Fail("抢课未开始");
            if (DateTime.Now > quota.EndTime) return Result<long>.Fail("抢课已结束");
            //库存预热
            var ttl = quota.EndTime - DateTime.Now + TimeSpan.FromHours(1);
            await _cache.SetIfNotExistsAsync($"seckill:stock:{courseId}", quota.Stock.ToString(),ttl,cancellationToken);
            //Lua原子校验
            var seckillResult = await _seckill.TryDeductAsync(courseId, userId, cancellationToken);
            if(seckillResult == SeckillResult.SoldOut) return Result<long>.Fail("名额不足");
            if (seckillResult == SeckillResult.Repeat) return Result<long>.Fail("请勿重复抢课");
            //异步化
            var message = new SeckillOrderMessage
            {
                CourseId = courseId,
                UserId = userId
            };
            await _publisher.PublishAsync(message, cancellationToken);
            await _cache.SetStringAsync(SeckillStatusKey.For(courseId, userId), "Pending", TimeSpan.FromMinutes(15), cancellationToken);
            //0=已经进入排队 订单由消费者异步生成
            return Result<long>.Ok(0);
        }
    }
}
