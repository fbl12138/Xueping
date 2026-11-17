using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Application.Services;

namespace XuePing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly ICreateReviewAsync _createReview;
        private readonly IDeleteReviewAsync _deleteReview;
        private readonly IGetReviewByCourseAsync _getReview;
        public ReviewController(ICreateReviewAsync createReview, IDeleteReviewAsync deleteReview, IGetReviewByCourseAsync getReview)
        {
            _createReview = createReview;
            _deleteReview = deleteReview;
            _getReview = getReview;
        }

        [HttpPost]
        public async Task<ActionResult<Result<ReviewDto>>> CreateReview([FromBody] CreateReviewRequest request, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var review = await _createReview.ExecuteAsync(request, userId, cancellationToken);
            return Ok(review);

        }
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<Result<bool>>> DeleteReview(long id,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
          return Ok( await _deleteReview.ExecuteAsync(id, userId, cancellationToken));
     
        }
        [HttpGet("course/{courseId:int}")]
        public async Task<ActionResult<Result<List<ReviewDto>>>> GetReview(int courseId,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if(!long.TryParse(userIdClaim,out var userId))
                return Unauthorized();
            return Ok(await _getReview.ExecuteAsync(courseId, userId, cancellationToken));
        }
    }
}
