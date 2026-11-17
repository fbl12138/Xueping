using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XuePing.Application.Services;

namespace XuePing.Api.HostedServices
{
    public class SeckillOrderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SeckillOrderWorker> _logger;

        public SeckillOrderWorker(IServiceScopeFactory scopeFactory, ILogger<SeckillOrderWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<ISeckillOrderConsumer>();
                    await consumer.ConsumeOnceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "消费抢课消息异常");
                }

                await Task.Delay(200, stoppingToken); // 空转节流，别烧 CPU
            }
        }
    }
}