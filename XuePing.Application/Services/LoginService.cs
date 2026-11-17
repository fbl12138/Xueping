using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using XuePing.Domain.Entities;
using Microsoft.Extensions.Options;
using XuePing.Domain.Common;


namespace XuePing.Application.Services
{
    public class LoginService : BaseService,ILoginAsync
    {
        private readonly ICacheService _cache;
        private readonly ITokenService _token;
        private readonly JwtOptions _jwtOptions;
        public LoginService(IXuePingDbContext dbContext,ICacheService cache,ITokenService token,IOptions<JwtOptions> jwtOptions ) : base( dbContext )
        {
    
            _cache = cache;
            _token = token;
            _jwtOptions = jwtOptions.Value;
        }
        public async Task<Result<LoginResponseDto>> ExecuteAsync(LoginRequest request,CancellationToken cancellationToken = default)
        {
            var code = await _cache.GetStringAsync($"code:{request.Phone}", cancellationToken);
            if(code is null || code != request.Code)
            {
                return Result<LoginResponseDto>.Fail("验证码错误或过期");
            }
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Phone == request.Phone, cancellationToken);
            if (user is null)
            {
                user = new User
                {
                    Phone = request.Phone,
                    NickName = request.Phone,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                DbContext.Users.Add(user);
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            var newtoken = _token.GenerateToken(user.Id, user.NickName);
            await _cache.SetStringAsync($"login:token:{user.Id}", newtoken, TimeSpan.FromMinutes(_jwtOptions.ExpireMinutes), cancellationToken);
            return Result<LoginResponseDto>.Ok(new LoginResponseDto
            {
                UserId = user.Id,
                Token = newtoken,
                NickName = user.NickName,
                Icon = user.Icon
            });
        }
    }
}
