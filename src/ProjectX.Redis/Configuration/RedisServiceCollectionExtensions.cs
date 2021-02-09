using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Redis.Abstractions;
using ProjectX.Redis.Implementations;
using StackExchange.Redis.Extensions.System.Text.Json;
using System;

namespace ProjectX.Redis.Configuration
{
    public static class RedisServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("RedisOptions").Get<RedisOptions>();
            if (options == null || options.Server == null)
                throw new ArgumentNullException("Redis options.");

            services.AddTransient<SystemTextJsonSerializer>();
            services.Configure<RedisOptions>(configuration);
            services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(options.Server);
            services.AddSingleton<IDefaultRedisClient, DefaultRedisClient>();
            return services;
        }
    }
}
