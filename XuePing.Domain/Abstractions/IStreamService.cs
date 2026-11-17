using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Common;

namespace XuePing.Domain.Abstractions
{
    public interface IStreamService
    {
        //XADD 追加消息 返回消息ID
        Task<string> PublishAsync(string stream, string field, string value,int maxLength = 1000, CancellationToken cancellationToken = default);
        //XREADGROUP 消费组内读消息(只读为投递给消费者的消息)，组不存在时自动建立
        Task<List<StreamMessage>> ReadGroupAsync(string stream, string group, string consumer, int count, CancellationToken cancellationToken = default);
        //读消费者名下擅未被ACK的Pending消息
        Task<List<StreamMessage>> ReadPendingAsync(string stream, string group, string consumer, int count, CancellationToken cancellationToken = default);

        //查看Pending明细 包含投递次数 闲置时间 用来判断重试还是进死信
        Task<List<PendingMessage>> GetPendingDetailsAsync(string stream, string group,string consumer,int count, CancellationToken cancellationToken = default);
        //接管别的消费者闲置超过MinIdleMs的消息
        Task<List<StreamMessage>> ClaimPendingAsync(string stream, string group, string consumer, long minIdleMs, int count, CancellationToken cancellationToken = default);

        //XACK确认一组消息处理完成
        Task AcknowledgeAsync(string stream, string group, IEnumerable<string> messageIds, CancellationToken cancellationToken = default);
        //汇总 组内待确认总数
        Task<long> GetPendingCountAsync(string stream, string group, CancellationToken cancellationToken = default);
      
    }
}
