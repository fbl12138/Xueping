using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XuePing.Domain.Common
{
    public class PendingMessage
    {
        public string Id { get; set; } = string.Empty; //消息Id
        public string Consumer { get; set; } = string.Empty; //当前归属消费者
        public long IdleMs { get; set; } //闲置时间(多久没人处理)
        public int DeliveryCount { get; set; }//重复次数
    }
}
