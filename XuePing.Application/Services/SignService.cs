using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Abstractions;
using XuePing.Application.Common;

namespace XuePing.Application.Services
{
    public class SignService : BaseService,ISignAsync
    {
        private readonly ISignService _sign;
        public SignService(IXuePingDbContext dbContext, ISignService sign) : base(dbContext) 
        {
            _sign = sign;
         }
        public async Task<Result<bool>> ExecuteAsync(long userId,CancellationToken cancellationToken = default)
        {
            var result = await _sign.SignAsync(userId, cancellationToken);
            if (!result) return Result<bool>.Fail("今天已经签到过了");
            return Result<bool>.Ok(result);
        }
        
    }
}
