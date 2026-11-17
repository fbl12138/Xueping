using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XuePing.Application.Services;
using Microsoft.Identity.Client;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SignController : ControllerBase
    {
        private readonly ISignAsync _sign;
        private readonly IGetSignStatusAsync _getSign;
        public SignController(ISignAsync sign, IGetSignStatusAsync getSign)
        {
            _sign = sign;
            _getSign = getSign;
        }
        [HttpPost]
        public async Task<ActionResult<Result<bool>>> Sign(CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId)) 
            return Unauthorized();
            return await _sign.ExecuteAsync(userId, cancellationToken);
        
        }
        [HttpGet("status")]
        public async Task<ActionResult<Result<SignStatusDto>>> GetStatus(CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            return await _getSign.ExecuteAsync(userId, cancellationToken);

        }

    }
}
