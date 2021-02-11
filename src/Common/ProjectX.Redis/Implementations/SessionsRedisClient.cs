using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Abstractions;
using ProjectX.Redis.Abstractions;

namespace ProjectX.Redis.Implementations
{
    public class SessionsRedisClient : BaseRedisClient<DB1>, ISessionsRedisClient
    {
        public SessionsRedisClient(IRedisCacheClient redisClient,
            ILoggerFactory loggerFactory,
            IOptions<RedisOptions> redisOptions)
            : base(redisClient, loggerFactory, redisOptions)
        {
        }
    }
}
