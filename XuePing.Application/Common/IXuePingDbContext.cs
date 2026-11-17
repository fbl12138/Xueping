using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using XuePing.Domain.Entities;

namespace XuePing.Application.Common
{
    public interface IXuePingDbContext
    {
        DbSet<Course> Courses { get; }
        DbSet<User> Users { get; }
        DbSet<CourseCategory> CourseCategories { get; }

        DbSet<Review> Reviews { get; }
        DbSet<CourseSelection> CourseSelections { get; }
        DbSet<SeckillQuota> SeckillQuotas { get; }
        DbSet<Follow> Follows { get; }
        DbSet<Post> Posts { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
