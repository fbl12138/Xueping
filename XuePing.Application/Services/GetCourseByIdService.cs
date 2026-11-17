using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;
using Microsoft.EntityFrameworkCore.SqlServer;
using XuePing.Domain.Common;
using System.IO.Pipelines;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Logging;

namespace XuePing.Application.Services
{
    public class GetCourseByIdService : BaseService, IGetCourseByIdAsync

    {
        private readonly ICourseUvService _uv;
        private readonly ICacheService _cache;
        private readonly ILogger<GetCourseByIdService> _logger;
        private static readonly Random _random = new();
        private readonly IServiceScopeFactory _scopeFactory;
        public GetCourseByIdService(IXuePingDbContext dbContext, ICacheService cache,IServiceScopeFactory scopeFactory,ICourseUvService uv,ILogger<GetCourseByIdService> logger) : base(dbContext)
        { _cache = cache;
            _scopeFactory = scopeFactory;
            _uv = uv;
            _logger = logger;
        }
        public async Task<Result<CourseDto>> ExecuteAsync(int id, long userId,CancellationToken cancellationToken = default)
        {
            var cacheKey = $"course:detail:{id}";
            try
            {
                await _uv.RecordAsync(id, userId, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "记录课程UV失败 courseId={CourseId} userId={UserId}",id, userId);
            }

            var redisData = await _cache.GetAsync<RedisData<CourseDto>>(cacheKey);
            if (redisData is not null)
            {
                if (redisData.Data.Id == 0)
                {
                    return Result<CourseDto>.Fail("课程不存在");
                }
                if (redisData.ExpireTime > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                { return Result<CourseDto>.Ok(redisData.Data); }
                _ = Task.Run(() => RebuildCacheAsync(cacheKey, id));
                return Result<CourseDto>.Ok(redisData.Data);

            }
            var result =  await GetDbCourse(id);
            return result;
            
        }
            private async Task RebuildCacheAsync(string cacheKey, int id, CancellationToken cancellationToken = default)
        {
            var lockKey = $"lock:course:{id}";
            var lockValue = Guid.NewGuid().ToString();
            var getlock = await _cache.SetIfNotExistsAsync(lockKey, lockValue, TimeSpan.FromSeconds(5), cancellationToken);
            if (getlock)
            {
                try
                {   
                    var newcourse = await _cache.GetAsync<RedisData<CourseDto>>(cacheKey);
                    if (newcourse is not null && newcourse.ExpireTime > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                    {
                        return;
                    }
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<IXuePingDbContext>();
                    var Dbcourse1 = await db.Courses.Where(c => c.Id == id).Select(p => new CourseDto
                    {
                        Id = p.Id,
                        CategoryId = p.CategoryId,
                        Name = p.Name,
                        Images = p.Images,
                        Area = p.Area,
                        Address = p.Address,
                        AvgScore = p.AvgScore,
                        CommentCount = p.CommentCount,
                        OpenHours = p.OpenHours
                    }).FirstOrDefaultAsync(cancellationToken);
                    if (Dbcourse1 is null)
                        return;
                    await SaveCourseWithLogicalExpireAsync(cacheKey, Dbcourse1, cancellationToken);
                }
                finally
                {
                    await _cache.RemoveAsync(lockKey);
                }
            }
            return;

        }
        private async Task<Result<CourseDto>> GetDbCourse(int id,CancellationToken cancellationToken = default)
        {
            var cacheKey = $"course:detail:{id}";
            var dbcourse = await DbContext.Courses.Where(c => c.Id == id).Select(p => new CourseDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                Name = p.Name,
                Images = p.Images,
                Area = p.Area,
                Address = p.Address,
                AvgScore = p.AvgScore,
                CommentCount = p.CommentCount,
                OpenHours = p.OpenHours
            }).FirstOrDefaultAsync(cancellationToken);
            if (dbcourse is null)
            { var emptyData = new RedisData<CourseDto>
            {
                Data = new CourseDto { Id = 0 },
                ExpireTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 2 * 60_000
            };
                await _cache.SetAsync(cacheKey, emptyData, TimeSpan.FromMinutes(3+_random.NextDouble()*10), cancellationToken);
                return Result<CourseDto>.Fail("课程不存在");
            }
            await SaveCourseWithLogicalExpireAsync(cacheKey, dbcourse, cancellationToken);
            return Result<CourseDto>.Ok(dbcourse);
        }
           
        private async Task SaveCourseWithLogicalExpireAsync(string cacheKey, CourseDto course, CancellationToken cancellationToken)
        {  
           
            var redisData = new RedisData<CourseDto>
            {
                Data = course,
                ExpireTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 30_000
            };
            await _cache.SetAsync(cacheKey, redisData, TimeSpan.FromMinutes(5+_random.NextDouble()*10), cancellationToken);
        }
        

    }

}


