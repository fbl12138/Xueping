using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.DTOs
{
    public class SeckillStatusDto
    {
        public int CourseId {  get; set; }
        public string Status { get; set; } = "None";
        //None = 没查到任何记录
        //Pending = 已经入队
        //Success = 已生成选课记录
        //Failed = 消费者重试3次仍然失败 进死信
        public long? OrderId { get; set; }
        public string? Message {  get; set; }
    }
}
