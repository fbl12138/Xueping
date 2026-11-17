using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design.Serialization;
using System.Security.AccessControl;
using XuePing.Application.DTOs;
using XuePing.Application.Services;

namespace XuePing.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ISendCodeAsync _sendCode;
        private readonly ILoginAsync _login;
        public UserController(ISendCodeAsync sendCode, ILoginAsync login)
        {
            _sendCode = sendCode;
            _login = login;
        }
        [AllowAnonymous]
        [HttpPost("code")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _sendCode.ExecuteAsync(request, cancellationToken);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken = default)
        {
            var result = await _login.ExecuteAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
