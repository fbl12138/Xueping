using XuePing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using XuePing.Application.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace XuePing.Infrastructure.Data
{
    public class XuePingDbContext : DbContext,IXuePingDbContext
    {
        public XuePingDbContext(DbContextOptions<XuePingDbContext> options)
            : base(options) { }


        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<CourseSelection> CourseSelections { get; set; }
        public DbSet<SeckillQuota> SeckillQuotas { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseSelection>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<SeckillQuota>().Property(c=>c.CourseId).ValueGeneratedNever();
            modelBuilder.Entity<Review>().HasIndex(c=>c.CourseId);
            modelBuilder.Entity<SeckillQuota>().HasKey(c => c.CourseId);
            modelBuilder.Entity<CourseSelection>().HasIndex(c=>new { c.UserId,c.CourseId}).IsUnique();
            modelBuilder.Entity<Follow>().HasIndex(c => new { c.UserId, c.FollowUserId }).IsUnique();
            modelBuilder.Entity<Post>().HasIndex(c => c.UserId);
            modelBuilder.Entity<Post>().Property(c => c.Content).HasMaxLength(1000);
            
            

        }
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Database.BeginTransactionAsync(cancellationToken);
        }
    }

}
