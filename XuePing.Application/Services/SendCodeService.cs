using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class SendCodeService :BaseService,ISendCodeAsync
    {
        private readonly ICacheService _cache;
        public SendCodeService (ICacheService cache,IXuePingDbContext dbContext) : base (dbContext)
        {
            _cache = cache;
        }
        public async Task<Result<string>> ExecuteAsync(SendCodeRequest request , CancellationToken cancellationToken = default)
        {
            var code = Random.Shared.Next(100000,1000000).ToString();
            await _cache.SetStringAsync($"code:{request.Phone}", code, TimeSpan.FromMinutes(5), cancellationToken);
            return Result<string>.Ok(code);
        }
    }
}
