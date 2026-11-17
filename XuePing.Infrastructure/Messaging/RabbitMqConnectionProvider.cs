using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Infrastructure.Messaging
{
   public sealed class RabbitMqConnectionProvider : IAsyncDisposable
    {
        private readonly ConnectionFactory _factory;
        //相当于一个异步锁只允许同时存在一个初始为1最大为1
        private readonly SemaphoreSlim _gate = new(1, 1);
        private IConnection? _connection;
        public RabbitMqConnectionProvider(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["MessageBus:HostName"] ?? "localhost",
                Port = int.TryParse(configuration["MessageBus:Port"], out var port) ? port : 5672,
                UserName = configuration["MessageBus:UserName"] ?? "guest",
                Password = Environment.GetEnvironmentVariable("RABBIT_PASS") ?? "guest", //密码仅从环境变量中来
                VirtualHost = configuration["MessageBus:VirtualHost"] ?? "/"
            };
          
        }
        public async Task<IConnection> GetAsync(CancellationToken cancellationToken = default)
        {
            if (_connection is { IsOpen: true }) return _connection; //已经存在连接且没有断 直接返回
            await _gate.WaitAsync(cancellationToken);
            try
            {
                if(_connection is not { IsOpen : true})
                {
                    _connection = await _factory.CreateConnectionAsync(cancellationToken);
                }
                return _connection;
            }
            finally
            {
                _gate.Release();//无论成功失败都释放锁
            }
        }
        public async ValueTask DisposeAsync()
        {
            if(_connection is not null) await _connection.DisposeAsync();
            _gate.Dispose();
        }

    }
}
