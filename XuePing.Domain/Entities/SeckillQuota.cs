using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Entities
{
    public class SeckillQuota
    {
        public int CourseId {  get; set; }
        public int Stock {  get; set; }
        public DateTime BeginTime {  get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
