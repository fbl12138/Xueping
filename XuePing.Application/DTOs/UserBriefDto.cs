using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class UserBriefDto
    {
        public long Id { get; set; }
        public string NickName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
