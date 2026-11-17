using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Entities;

namespace XuePing.Application.Services
{
    public class CreateCourseService : BaseService, ICreateCourseAsync
    {
        public CreateCourseService(IXuePingDbContext dbContext) : base(dbContext) { }
        public async Task<Result<CourseDto>> ExecuteAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
        {
            var courseCreate = new Course
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Images = request.Images,
                Area = request.Area,
                Address = request.Address,
                OpenHours = request.OpenHours,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now

            };
            DbContext.Courses.Add(courseCreate);
            await DbContext.SaveChangesAsync(cancellationToken);
            var dto = new CourseDto
            {
                Id = courseCreate.Id,
                CategoryId = courseCreate.CategoryId,
                Name = courseCreate.Name,
                Images = courseCreate.Images,
                Area = courseCreate.Area,
                Address = courseCreate.Address,
                AvgScore = courseCreate.AvgScore,
                CommentCount = courseCreate.CommentCount,
                OpenHours = courseCreate.OpenHours
            };
            return Result<CourseDto>.Ok(dto);

        }
    }
}
