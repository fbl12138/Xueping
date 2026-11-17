using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Application.Services;
using XuePing.Domain.Abstractions;

namespace XuePing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly IFollowUserAsync _followuser;
        private readonly IUnfollowUserAsync _unfollowuser;
        private readonly IIsFollowingAsync _isfollowing;
        private readonly IGetCommonFollowsAsync _getcommonfollows;
        public FollowController(IFollowUserAsync followuser, IUnfollowUserAsync unfollowuser, IIsFollowingAsync isfollowing, IGetCommonFollowsAsync getcommonfollows)
        {
            _followuser = followuser;
            _unfollowuser = unfollowuser;
            _isfollowing = isfollowing;
            _getcommonfollows = getcommonfollows;
        }
        //点赞
        [HttpPost("{followUserId:long}")]
        public async Task<ActionResult<Result<bool>>> Follow([FromRoute]long followUserId,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if(!long.TryParse(userIdClaim,out var userId))
                return Unauthorized();
          return await _followuser.ExecuteAsync(userId, followUserId, cancellationToken);

        }
        //取消点赞
        [HttpDelete("{followUserId:long}")]
        public async Task<ActionResult<Result<bool>>> UnFollow([FromRoute]long followUserId,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim,out var userId))
                return Unauthorized();
            return await _unfollowuser.ExecuteAsync(userId, followUserId, cancellationToken);
        }
        //是否点赞
        [HttpGet("isfollow/{followUserId:long}")]
        public async Task<ActionResult<Result<bool>>> IsFollow([FromRoute] long followUserId,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return await _isfollowing.ExecuteAsync(userId,followUserId, cancellationToken);
        }
        //获取共同点赞
        [HttpGet("common/{otherUserId:long}")]
        public async Task<ActionResult<Result<List<UserBriefDto>>>> CommonFollow([FromRoute]long otherUserId,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return await _getcommonfollows.ExecuteAsync(userId, otherUserId, cancellationToken);
        }
    }
}
