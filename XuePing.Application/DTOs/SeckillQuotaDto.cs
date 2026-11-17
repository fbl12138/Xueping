using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class SeckillQuotaDto
    {
        
        public int CourseId { get; set; }
        public int Stock { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Started { get; set; }
        public bool Ended { get; set; }
    }
}
