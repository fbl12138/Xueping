using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Common
{
    public class RedisData<T>
    {
        public T Data { get; set; } = default!;
        public long ExpireTime {  get; set; }
    }
}
