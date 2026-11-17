using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Linq;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace XuePing.Application.Services
{
    public class UpdateCourseService : BaseService,IUpdateCourseAsync
    {
        private readonly ICacheService _cache;
        public UpdateCourseService(IXuePingDbContext dbContext, ICacheService cache) : base(dbContext) { _cache = cache; }
        public async Task<Result<CourseDto>> ExecuteAsync(int id,UpdateCourseRequest request,CancellationToken cancellationToken = default)
        {
            
            var Dbcourse = await DbContext.Courses.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (Dbcourse == null)
            {
                return Result<CourseDto>.Fail("课程不存在");
            }
            Dbcourse.CategoryId = request.CategoryId;
            Dbcourse.Name = request.Name;
            Dbcourse.Images = request.Images;
            Dbcourse.Area = request.Area;
            Dbcourse.Address = request.Address;
            Dbcourse.OpenHours = request.OpenHours;
            Dbcourse.UpdatedAt = DateTime.Now;
            await DbContext.SaveChangesAsync(cancellationToken); 
            await _cache.RemoveAsync($"course:detail:{id}", cancellationToken);
            var courseDto = new CourseDto
            {
                Id = Dbcourse.Id,
                CategoryId = Dbcourse.CategoryId,
                Name = Dbcourse.Name,
                Images = Dbcourse.Images,
                Area = Dbcourse.Area,
                Address = Dbcourse.Address,
                AvgScore = Dbcourse.AvgScore,
                CommentCount = Dbcourse.CommentCount,
                OpenHours = Dbcourse.OpenHours
            };
      
            return Result<CourseDto>.Ok(courseDto);
        }
    }
}
