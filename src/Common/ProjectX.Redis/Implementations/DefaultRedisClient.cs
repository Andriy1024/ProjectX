﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Abstractions;
using ProjectX.Redis.Abstractions;

namespace ProjectX.Redis.Implementations
{
    public class DefaultRedisClient : BaseRedisClient<DB0>, IDefaultRedisClient
    {
        public DefaultRedisClient(IRedisCacheClient redisClient,
            ILoggerFactory loggerFactory,
            IOptions<RedisOptions> redisOptions)
            : base(redisClient, loggerFactory, redisOptions)
        {
        }
    }
}
