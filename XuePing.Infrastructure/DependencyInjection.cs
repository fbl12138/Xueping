using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.ClientModel.Primitives;
using XuePing.Application.Common;
using XuePing.Application.Services;
using XuePing.Domain.Abstractions;
using XuePing.Infrastructure.Caching;
using XuePing.Infrastructure.Data;
using XuePing.Infrastructure.Messaging;
using XuePing.Infrastructure.Token;

namespace XuePing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<XuePingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));
        services.AddScoped<IXuePingDbContext>(sp =>
            sp.GetRequiredService<XuePingDbContext>());
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        services.AddScoped<ICacheService,RedisCacheService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ISeckillStockService,RedisSeckillStockService>();
        services.AddScoped<IIdGenerator, RedisIdGenerator>();
        services.AddScoped<IReviewLikeService, RedisReviewLikeService>();
        services.AddScoped<IFollowService, RedisFollowService>();
        services.AddScoped<IFeedService, RedisFeedService>();
        services.AddScoped<ISignService, RedisSignService>();
        services.AddScoped<ICourseUvService, RedisCourseUvService>();
        services.AddScoped<IStreamService, RedisStreamService>();
        services.AddSingleton<RabbitMqConnectionProvider>();                            // 单例连接，发布者与消费者共用
        if (string.Equals(configuration["MessageBus:Provider"], "RabbitMQ", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<ISeckillOrderPublisher, RabbitSeckillOrderPublisher>();  // RabbitMQ 发布者
            services.AddSingleton<ISeckillOrderConsumer, RabbitSeckillOrderConsumer>();   // RabbitMQ 消费者
        }
        else
        {
            services.AddScoped<ISeckillOrderPublisher, RedisSeckillOrderPublisher>();     // 原 Stream 发布者
            services.AddScoped<ISeckillOrderConsumer, SeckillOrderConsummerService>();    // 原 Stream 消费者
        }
        return services;
    }
}