using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using XuePing.Domain.Abstractions;
using XuePing.Domain.Common;

namespace XuePing.Infrastructure.Caching
{
    public class RedisSeckillOrderPublisher : ISeckillOrderPublisher
    {
        private const string StreamKey = "stream:seckill:order";
        private const string Field = "order";
        private readonly IStreamService _stream;
        public RedisSeckillOrderPublisher(IStreamService stream)
        {
            _stream = stream;
        }
        public async Task PublishAsync(SeckillOrderMessage message,CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(message);
            await _stream.PublishAsync(StreamKey, Field, json, cancellationToken: cancellationToken);
        }
    }
}
