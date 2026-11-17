using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Common;
using System.Security.Claims;

namespace XuePing.Infrastructure.Token
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _options;
        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public string GenerateToken(long userId,string nickName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("userId",userId.ToString()),
                new Claim(ClaimTypes.Name,nickName)
            };
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_options.ExpireMinutes),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
