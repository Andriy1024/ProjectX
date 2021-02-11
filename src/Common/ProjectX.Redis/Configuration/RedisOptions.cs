using StackExchange.Redis.Extensions.Core.Configuration;

namespace ProjectX.Redis
{
    public class RedisOptions
    {
        public RedisConfiguration Server { get; set; }

        // Default distributed lock options
        public int TakeRetryDelayMilliseconds { get; set; }
        public int DefaultWaitLockMiliseconds { get; set; }
        public int DefaultLockTimeMiliseconds { get; set; }
    }
}
