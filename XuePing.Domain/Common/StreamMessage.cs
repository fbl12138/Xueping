using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XuePing.Domain.Common
{
    public class StreamMessage
    {
        public string Id { get; set; } = string.Empty; //消息ID
        public string Field { get; set; } = string.Empty;//字段名
        public string Value { get; set; } = string.Empty;//字段值
    }
}
