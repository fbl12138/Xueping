using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuePing.Application.Services;
using XuePing.Application.Common;
using Microsoft.Identity.Client;
using XuePing.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using XuePing.Application.DTOs;

namespace XuePing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CourseSelectionController : ControllerBase
    {
        private readonly ISelectCourseAsync _selectCourseAsync;
        private readonly IGetSeckillQuotaAsync _getSeckillQuota;
        private readonly IGetSeckillStatusAsync _getSeckillStatus;
        public CourseSelectionController(ISelectCourseAsync selectCourseAsync, IGetSeckillQuotaAsync getSeckillQuota, IGetSeckillStatusAsync getSeckillStatus)
        {
            _selectCourseAsync = selectCourseAsync;
            _getSeckillQuota = getSeckillQuota;
            _getSeckillStatus = getSeckillStatus;
        }
        [HttpPost("quota/{courseId:int}")]
        public async Task<ActionResult<Result<long>>> GetQuota(int courseId,CancellationToken cancellationToken = default)
        {
           return Ok(await _getSeckillQuota.ExecuteAsync(courseId, cancellationToken));
        }
        [HttpPost("{courseId:int}")]
        public async Task<ActionResult<Result<long>>> Select(int courseId, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _selectCourseAsync.ExecuteAsync(courseId, userId, cancellationToken);
            return Ok(result);
        }
        [HttpGet("status/{courseId:int}")]
        public async Task<ActionResult<Result<SeckillStatusDto>>> GetStatus(int courseId,CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            var result = await _getSeckillStatus.ExecuteAsync(courseId, userId, cancellationToken);
            return Ok(result);                                        
        }

    }
}
