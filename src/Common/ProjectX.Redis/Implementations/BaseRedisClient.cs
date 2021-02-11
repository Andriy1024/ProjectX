using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectX.Redis.Abstractions;

namespace ProjectX.Redis.Implementations
{
    public abstract class BaseRedisClient<TDataBase> : IRedisClient
        where TDataBase : DbNumber, new()
    {
        readonly static IEnumerable<RedisKey> _emptyKeys = Array.Empty<RedisKey>();

        readonly RedisOptions _redisOptions;
        readonly IRedisDatabase _database;
        readonly ILoggerFactory _loggerFactory;

        readonly TimeSpan _defaultLockTimeMiliseconds;
        readonly TimeSpan _defaultWaitLockMilisecondsLockTimeMiliseconds;
        readonly TimeSpan _defaulTakeRetryDelayMilliseconds;

        public BaseRedisClient(IRedisCacheClient redisClient,
            ILoggerFactory loggerFactory,
            IOptions<RedisOptions> redisOptions)
        {
            AssertNotNull(redisClient, nameof(redisClient));
            AssertNotNull(redisClient, nameof(redisOptions));
            AssertNotNull(loggerFactory, nameof(loggerFactory));

            _redisOptions = redisOptions.Value;
            _database = redisClient.GetDb(new TDataBase().Value);

            _defaulTakeRetryDelayMilliseconds = TimeSpan.FromMilliseconds(_redisOptions.TakeRetryDelayMilliseconds);
            _defaultWaitLockMilisecondsLockTimeMiliseconds = TimeSpan.FromMilliseconds(_redisOptions.DefaultWaitLockMiliseconds);
            _defaultLockTimeMiliseconds = TimeSpan.FromMilliseconds(_redisOptions.DefaultLockTimeMiliseconds);
        }

        private void AssertNotNull<T>(T item, string message)
        {
            if (item == null)
                throw new ArgumentNullException(message);
        }

        public Task<bool> AddStringAsync(RedisKey key, string item, TimeSpan? expireIn = null)
        {
            return _database.Database.StringSetAsync(key.Value, item, expireIn);
        }

        public Task<bool> AddAsync<TIn>(RedisKey key, TIn item)
        {
            return _database.AddAsync(key, item);
        }

        public Task<bool> AddAsync<TIn>(RedisKey key, TIn item, TimeSpan expiresIn)
        {
            return _database.AddAsync(key, item, expiresIn);
        }

        public Task<bool> DeleteAsync(RedisKey key)
        {
            return _database.RemoveAsync(key);
        }

        public Task<long> DeleteAsync(IEnumerable<RedisKey> keys)
        {
            return _database.RemoveAllAsync(keys.Cast<string>());
        }

        public async Task<long> DeleteByPatternAsync(RedisKey pattern)
        {
            var keys = await SearchKeysAsync(pattern);
            if (keys == null || !keys.Any())
                return 0;

            return await DeleteAsync(keys);
        }

        public Task<bool> ExistsAsync(RedisKey key)
        {
            return _database.ExistsAsync(key);
        }

        public async Task<IDictionary<RedisKey, T>> GetAllAsync<T>(IEnumerable<RedisKey> keys)
        {
            var result = await _database.GetAllAsync<T>(keys.Cast<string>());
            if (result == null || !result.Any())
                return new Dictionary<RedisKey, T>(0);

            return new Dictionary<RedisKey, T>(result.Select(v => new KeyValuePair<RedisKey, T>(v.Key, v.Value)));
        }

        public async Task<string> GetStringAsync(RedisKey key)
        {
            return await _database.Database.StringGetAsync(key.Value);
        }

        public Task<TOut> GetAsync<TOut>(RedisKey key)
        {
            return _database.GetAsync<TOut>(key);
        }

        public async Task<TOut> GetOrAddAsync<TOut>(RedisKey key, TimeSpan expiresIn, Func<Task<TOut>> factory)
        {
            TOut item = await _database.GetAsync<TOut>(key);
            if (item != null)
                return item;

            item = await factory();
            await _database.AddAsync(key, item, expiresIn);
            return item;
        }

        public async Task<TOut> GetOrAddAsync<TOut>(RedisKey key, TimeSpan expiresIn, Func<TOut> factory)
        {
            TOut item = await _database.GetAsync<TOut>(key);
            if (item != null)
                return item;

            item = factory();
            await _database.AddAsync(key, item, expiresIn);
            return item;
        }


        public RedisLock Lock(RedisKey key, TimeSpan? expiresIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null)
        {
            expiresIn ??= _defaultLockTimeMiliseconds;
            waitTime ??= _defaultWaitLockMilisecondsLockTimeMiliseconds;
            retryTime ??= _defaulTakeRetryDelayMilliseconds;

            var logger = _loggerFactory.CreateLogger<RedisLock>();
            var redisLock = new RedisLock(_database.Database,
                            logger,
                            key: key,
                            expiryTime: expiresIn.Value,
                            waitTime: waitTime,
                            retryTime: retryTime);

            redisLock.Start();
            return redisLock;
        }

        /// <summary>
        /// Try take lock in one attempt.
        /// </summary>
        public RedisLock TryLock(RedisKey key, TimeSpan? expiresIn = null)
        {
            expiresIn ??= _defaultLockTimeMiliseconds;
            var logger = _loggerFactory.CreateLogger<RedisLock>();
            var redisLock = new RedisLock(_database.Database,
                            logger,
                            key: key,
                            expiryTime: expiresIn.Value,
                            waitTime: null,
                            retryTime: null);

            redisLock.Start();
            return redisLock;
        }

        /// <summary>
        /// Try take lock in one attempt.
        /// </summary>
        public async Task<RedisLock> TryLockAsync(RedisKey key, TimeSpan? expiresIn = null)
        {
            expiresIn ??= _defaultLockTimeMiliseconds;
            var logger = _loggerFactory.CreateLogger<RedisLock>();
            var redisLock = new RedisLock(_database.Database,
                            logger,
                            key: key,
                            expiryTime: expiresIn.Value,
                            waitTime: null,
                            retryTime: null);

            await redisLock.StartAsync();
            return redisLock;
        }

        public async Task<RedisLock> LockAsync(RedisKey key, TimeSpan? expiresIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null, CancellationToken cancellationToken = default)
        {
            expiresIn ??= _defaultLockTimeMiliseconds;
            waitTime ??= _defaultWaitLockMilisecondsLockTimeMiliseconds;
            retryTime ??= _defaulTakeRetryDelayMilliseconds;

            var logger = _loggerFactory.CreateLogger<RedisLock>();
            var redisLock = new RedisLock(_database.Database,
                            logger,
                            key: key,
                            expiryTime: expiresIn.Value,
                            waitTime: waitTime,
                            retryTime: retryTime);

            await redisLock.StartAsync();
            return redisLock;
        }

        public RedisLock LockOrThrow(RedisKey key, TimeSpan? expiresIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null)
        {
            var redisLock = Lock(key, expiresIn, waitTime, retryTime);
            if (!redisLock.IsAcquired)
                throw new Exception($"Failet to get redisl lock for: {key}, lockTime: {expiresIn}, waitTime: {waitTime}, retryTime: {retryTime}");

            return redisLock;
        }

        public async Task<RedisLock> LockOrThrowAsync(RedisKey key, TimeSpan? expiresIn = null, TimeSpan? waitTime = null, TimeSpan? retryTime = null, CancellationToken cancellationToken = default)
        {
            var redisLock = await LockAsync(key, expiresIn, waitTime, retryTime);
            if (!redisLock.IsAcquired)
                throw new Exception($"Failet to get redisl lock for: {key}, lockTime: {expiresIn}, waitTime: {waitTime}, retryTime: {retryTime}");

            return redisLock;
        }

        public async Task<IEnumerable<RedisKey>> SearchKeysAsync(RedisKey pattern)
        {
            var result = await _database.SearchKeysAsync(pattern);
            if (result == null || !result.Any())
                return _emptyKeys;

            return result.Cast<RedisKey>().ToArray();
        }
    }
}
