using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace XuePing.Domain.Entities
{
    public class Post
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
