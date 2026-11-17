using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuePing.Application.DTOs;
using XuePing.Application.Services;
using XuePing.Application.Common;

namespace XuePing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedController : ControllerBase
    {
        private readonly IPublishPostAsync _publishPost;
        private readonly IGetFeedAsync _getFeed;
        public FeedController(IPublishPostAsync publishPost, IGetFeedAsync getFeed)
        {
            _publishPost = publishPost;
            _getFeed = getFeed;
        }
        //推送
        [HttpPost]
        public async Task<ActionResult<Result<PostDto>>> Push(CreatePostRequest request,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return await _publishPost.ExecuteAsync(userId, request, cancellationToken);
        }
        //查看最近的五个通知
        [HttpGet]
        public async Task<ActionResult<Result<FeedPageDto>>> Get(long? lastId = null,int offset = 0,int count = 5,CancellationToken cancellationToken = default)
        {
            count = Math.Clamp(count, 1, 10);
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return await _getFeed.ExecuteAsync(userId,lastId,offset, count, cancellationToken);
        }
    }
}
