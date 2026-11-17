using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class ReviewDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string NickName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Score { get; set; }
        public int LikeCount { get; set; }
        public bool Liked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
