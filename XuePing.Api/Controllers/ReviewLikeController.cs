using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using XuePing.Application.Services;
using XuePing.Application.Common;
using XuePing.Application.DTOs;


namespace XuePing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewLikeController : ControllerBase
    {
        private readonly ILikeReviewAsync _like;
        private readonly IGetReviewLikeRankAsync _top;
        private readonly IUnlikeReviewAsync _unlike;
        public ReviewLikeController(ILikeReviewAsync like, IUnlikeReviewAsync unlike,IGetReviewLikeRankAsync top)
        {
            _like = like;
            _unlike = unlike;
            _top = top;
        }
        [HttpPost("{reviewId:long}")]
        public async Task<ActionResult<Result<bool>>> Like(long reviewId,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return await _like.ExecuteAsync(reviewId, userId, cancellationToken);
            
        }
        [HttpDelete("{reviewId:long}")]
        public async Task<ActionResult<Result<bool>>> UnLike(long reviewId,CancellationToken cancellationToken = default)
        {

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return await _unlike.ExecuteAsync(reviewId, userId, cancellationToken);
        }
        [HttpGet("top/{reviewId:long}")]
        
        public async Task<ActionResult<Result<List<UserBriefDto>>>> GetTop([FromRoute] long reviewId,int count = 5,CancellationToken cancellationToken = default)
        {
            count = Math.Clamp(count, 1, 10);
            return await _top.ExecuteAsync(reviewId, count, cancellationToken);

        }
    }
}
