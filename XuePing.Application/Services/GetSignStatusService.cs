using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;
using XuePing.Domain.Abstractions;

namespace XuePing.Application.Services
{
    public class GetSignStatusService : BaseService, IGetSignStatusAsync
    {
        private readonly ISignService _sign;
        public GetSignStatusService(IXuePingDbContext dbContext,ISignService sign) : base(dbContext)
        {
            _sign = sign;
        }
        public async Task<Result<SignStatusDto>> ExecuteAsync(long userId,CancellationToken cancellationToken = default)
        {
            var signeddays = await _sign.GetSignedDaysAsync(userId, cancellationToken);
            var consecutivedays = await _sign.GetConsecutiveDaysAsync(userId, cancellationToken);
            return Result<SignStatusDto>.Ok(new SignStatusDto
            {
                SignedDays = signeddays,
                ConsecutiveDays = consecutivedays
            });
        }
    }
}
