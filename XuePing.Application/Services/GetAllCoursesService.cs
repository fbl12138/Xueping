using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public class GetAllCoursesService : BaseService,IGetAllCoursesAsync
    {
        public GetAllCoursesService(IXuePingDbContext dbContext) : base(dbContext) { }
        public async Task<Result<List<CourseDto>>> ExecuteAsync(CancellationToken cancellationToken=default)
        {
            var items = await DbContext.Courses.AsNoTracking().Select(p =>
                new CourseDto
                {   Id = p.Id,
                    CategoryId = p.CategoryId,
                    Name = p.Name,
                    Images = p.Images,
                    Area = p.Area,
                    Address = p.Address,
                    AvgScore = p.AvgScore,
                    CommentCount = p.CommentCount,
                    OpenHours = p.OpenHours

                }).ToListAsync(cancellationToken);
            return Result<List<CourseDto>>.Ok(items);
        }
        
    }
}
