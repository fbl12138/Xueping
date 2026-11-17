using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class FeedPageDto
    {
        public List<PostDto> List { get; set; } = new List<PostDto>();
        public long MinTime {  get; set; }
        public int Offset {  get; set; }
    }
}
