using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Domain.Entities;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;
using System.Reflection;

namespace XuePing.Application.Services
{
    public class PublishPostService :BaseService,IPublishPostAsync
    {
        private readonly IFeedService _feed;
        private readonly IFollowService _follow;
        private readonly ILogger<PublishPostService> _logger;
        public PublishPostService(IXuePingDbContext dbContext,IFeedService feed,IFollowService follow,ILogger<PublishPostService> logger):base(dbContext)
        {
            _follow = follow;
            _logger = logger;
            _feed = feed;
        }
        public async Task<Result<PostDto>> ExecuteAsync(long userId,CreatePostRequest request,CancellationToken cancellationToken = default)
        {
            var content = (request.Content ?? string.Empty).Trim();
            if (content.Length == 0) return Result<PostDto>.Fail("动态内容不能为空");
            if (content.Length > 1000) return Result<PostDto>.Fail("动态内容不能超过1000字");
            var scoreMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var post = new Post { UserId = userId, Content = content, CreatedAt = DateTime.Now };
            DbContext.Posts.Add(post);
            await DbContext.SaveChangesAsync(cancellationToken);
            try
            {
                var receivers = await _follow.GetFansIdsAsync(userId, cancellationToken);
                receivers.Add(userId);
                await _feed.PushAsync(receivers, post.Id, scoreMs, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "推送Feed失败 userId={UserId} postId={PostId}", userId,post.Id);
            }
            return Result<PostDto>.Ok(new PostDto
            {
                Id = post.Id,
                UserId = userId,
                Content = post.Content,
                CreatedAt = post.CreatedAt
            });
        }
    }
}
