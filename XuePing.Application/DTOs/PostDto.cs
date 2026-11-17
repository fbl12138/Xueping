using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class PostDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string NickName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
