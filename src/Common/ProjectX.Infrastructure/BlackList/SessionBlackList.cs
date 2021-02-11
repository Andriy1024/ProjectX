using System;
using System.Threading.Tasks;
using ProjectX.Redis.Abstractions;
using ProjectX.Redis;
using ProjectX.Core.BlackList;
using ProjectX.Core;

namespace ProjectX.Infrastructure.BlackList
{
    public class SessionBlackList : ISessionBlackList
    {
        readonly ISessionsRedisClient _redisClient;

        public SessionBlackList(ISessionsRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        public Task AddToBlackListAsync(string sessionId, TimeSpan timeSpan)
            => _redisClient.AddStringAsync(GetKey(sessionId), sessionId, timeSpan);

        public Task<bool> HasSessionAsync(string sessionId)
            => _redisClient.ExistsAsync(GetKey(sessionId));

        private RedisKey GetKey(string sessionId)
        {
            Utill.ThrowIfNullOrEmpty(sessionId, nameof(sessionId));

            return new RedisKey("black_list", sessionId);
        }
    }
}
