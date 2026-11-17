using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace XuePing.Domain.Entities
{
    public class Review
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int CourseId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Score { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
