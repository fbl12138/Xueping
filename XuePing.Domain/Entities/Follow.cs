using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Entities
{
    public class Follow
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long FollowUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
