using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Common
{
    public static class SeckillStatusKey
    {
        public static string For(int courseId, long userId) => $"seckill:status:{courseId}:{userId}";
    }
}
