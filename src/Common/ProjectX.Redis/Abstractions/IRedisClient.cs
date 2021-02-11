using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Redis.Abstractions
{
    public interface IRedisClient
    {
        Task<bool> AddAsync<TIn>(RedisKey key, TIn item);
        Task<bool> AddAsync<TIn>(RedisKey key, TIn item, TimeSpan expireIn);
        Task<bool> AddStringAsync(RedisKey key, string item, TimeSpan? expireIn = null);

        Task<bool> ExistsAsync(RedisKey key);
        Task<bool> DeleteAsync(RedisKey key);
        Task<long> DeleteAsync(IEnumerable<RedisKey> key);
        Task<long> DeleteByPatternAsync(RedisKey pattern);

        Task<TOut> GetAsync<TOut>(RedisKey key);
        Task<string> GetStringAsync(RedisKey key);
        Task<IDictionary<RedisKey, T>> GetAllAsync<T>(IEnumerable<RedisKey> keys);
        Task<IEnumerable<RedisKey>> SearchKeysAsync(RedisKey pattern);

        Task<TOut> GetOrAddAsync<TOut>(RedisKey RedisKey, TimeSpan expireIn, Func<Task<TOut>> factory);
        Task<TOut> GetOrAddAsync<TOut>(RedisKey RedisKey, TimeSpan expireIn, Func<TOut> factory);

        #region Distributed lock

        /// <summary>
        /// Try take lock during wait Time with retry Time intervals.
        /// </summary>
        RedisLock Lock(RedisKey key, TimeSpan? expiryIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null);

        /// <summary>
        /// Try get lock in one time, without additional attempts.
        /// </summary>
        RedisLock TryLock(RedisKey key, TimeSpan? expiresIn = null);

        /// <summary>
        /// Try take lock during wait Time with retry Time intervals.
        /// </summary>
        Task<RedisLock> LockAsync(RedisKey key, TimeSpan? expiryIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Try get lock in one time, without additional attempts.
        /// </summary>
        Task<RedisLock> TryLockAsync(RedisKey key, TimeSpan? expiresIn = null);

        /// <summary>
        /// Throw exception if couldn't take the lock.
        /// </summary>
        RedisLock LockOrThrow(RedisKey key, TimeSpan? expiryIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null);

        /// <summary>
        /// Throw exception if couldn't take the lock.
        /// </summary>
        Task<RedisLock> LockOrThrowAsync(RedisKey key, TimeSpan? expiryIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null, CancellationToken cancellationToken = default);

        #endregion
    }
}
