using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Domain.Common;

namespace XuePing.Domain.Abstractions
{
    public interface IFeedService
    {
        Task<List<FeedEntry>> GetFeedIdsAsync(long userId,long? lastId,int offset,int count, CancellationToken cancellationToken = default);
        Task PushAsync(IReadOnlyCollection<long> receiverIds, long postId, long scoreMs, CancellationToken cancellationToken = default);
    }
}
