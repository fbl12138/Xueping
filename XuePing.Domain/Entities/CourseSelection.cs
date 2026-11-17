using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Domain.Entities
{
    public class CourseSelection
    {
        public long Id { get; set; }
        public long UserId { get; set; } 
        public int CourseId {  get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
   
}
