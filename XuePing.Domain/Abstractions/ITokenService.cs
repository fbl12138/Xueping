using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Abstractions
{
    public interface ITokenService
    {
        string GenerateToken(long userId, string nickName);
    }
}
