using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class GetFeedService : BaseService,IGetFeedAsync
    {
        private readonly IFeedService _feed;
        public GetFeedService(IXuePingDbContext dbContext,IFeedService feed):base(dbContext)
        {
            _feed = feed;
        }
        public async Task<Result<FeedPageDto>> ExecuteAsync(long userId,long? lastId,int offset,int count,CancellationToken cancellationToken = default)
        {   if (offset < 0) offset = 0;
            var entries = await _feed.GetFeedIdsAsync(userId,lastId,offset, count, cancellationToken);
            if (entries.Count == 0) return Result<FeedPageDto>.Ok(new FeedPageDto());
            var ids = entries.Select(e=>e.PostId).ToList();
            var rows = await DbContext.Posts.Where(p => ids.Contains(p.Id)).Join(DbContext.Users
                , p => p.UserId, u => u.Id, (p, u) => new PostDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    NickName = u.NickName,
                    Icon = u.Icon,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt
                }).ToListAsync(cancellationToken);
            var map = rows.ToDictionary(r => r.Id);
            var ordered = ids.Where(map.ContainsKey).Select(id => map[id]).ToList();
            var minTime = entries[entries.Count - 1].Score;
            var sameScoreCount = 0;
            for(int i = entries.Count - 1;i>=0;i--)
            {
                if (entries[i].Score != minTime) break;
                sameScoreCount++;
            }
            return Result<FeedPageDto>.Ok(new FeedPageDto
            {
                List = ordered,
                MinTime = minTime,
                Offset = sameScoreCount
            });
           
        }

    }
}
