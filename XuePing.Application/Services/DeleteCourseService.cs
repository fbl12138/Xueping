using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public class DeleteCourseService :BaseService,IDeleteCourseAsync
    {
        public DeleteCourseService(IXuePingDbContext dbContext) : base(dbContext) { }
        public async Task<Result<bool>> ExecuteAsync(int id, CancellationToken cancelationToken = default)
        {
            var course = await DbContext.Courses.FirstOrDefaultAsync(p => p.Id == id, cancelationToken);
            if (course == null)
            {
                return Result<bool>.Fail("课程不存在");
            }
            DbContext.Courses.Remove(course);
            await DbContext.SaveChangesAsync();
            return Result<bool>.Ok(true);
        }
    }
}
