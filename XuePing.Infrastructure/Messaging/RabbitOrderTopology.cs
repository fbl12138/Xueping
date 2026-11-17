using RabbitMQ.Client;
using System.Collections.Generic;

namespace XuePing.Infrastructure.Messaging
{
    public static class RabbitOrderTopology
    {
        public const string QueueName = "seckill.order.queue";

        public const string RetryExchange = "seckill.order.retry.ex"; //重试交换机
        public const string DeadExchange = "seckill.order.dlx";//死信交换机

        public const string RetryQueue = "seckill.order.retry";//重试队列
        public const string DeadQueue = "seckill.order.dlq";//死信队列

        public const string RetryRoutingKey = "seckill.order.retry";
        public const string DeadRoutingKey = "seckill.order.dead";//死信路由键
        public static async Task DeclareAsync(IChannel channel,CancellationToken cancellationToken = default)
        {
            await channel.ExchangeDeclareAsync(
                exchange: RetryExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);
            //在重试队列设置 消息在重试队列里面待3秒后 按照路由键绑定的队列重新投递是重新创造一个相同的消息投递
            await channel.QueueDeclareAsync(
                queue: RetryQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    ["x-message-ttl"] = 3000,
                    ["x-dead-letter-exchange"] = "",
                    ["x-dead-letter-routing-key"] = QueueName
                },
                cancellationToken:cancellationToken);
            await channel.QueueBindAsync(
                queue: RetryQueue,
                exchange: RetryExchange,
                routingKey: RetryRoutingKey,
                cancellationToken: cancellationToken
                );
            await channel.QueueDeclareAsync(                                            // 终死队列：普通队列，不再挂 DLX
              queue: DeadQueue,
              durable: true,
              exclusive: false,
              autoDelete: false,
              cancellationToken: cancellationToken);
            await channel.QueueBindAsync(
                queue: DeadQueue,
                exchange: RetryExchange,
                routingKey: DeadRoutingKey,
                cancellationToken: cancellationToken);
           await channel.QueueDeclareAsync(                                            // 主队列：DLX 现在指向重试队列
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    ["x-dead-letter-exchange"] = RetryExchange,                         // 失败消息交给重试交换机
                    ["x-dead-letter-routing-key"] = RetryRoutingKey                   // 用 retry 键 → 进重试队列等 3 秒
                },
                cancellationToken: cancellationToken);
        }
    }

}
