using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class SignStatusDto
    {
        public List<int> SignedDays { get; set; } = new List<int>();
        public int ConsecutiveDays { get; set; }
    }
}
