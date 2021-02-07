using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ProjectX.Redis.Utill;
using RedisKey = ProjectX.Redis.RedisKey;

namespace ProjectX.Redis
{
    /// <summary>
    /// Source: https://github.com/samcook/RedLock.net
    /// </summary>
    public sealed class RedisLock : IDisposable, IAsyncDisposable
    {
        private readonly object _lockObject = new object();
        private readonly SemaphoreSlim _extendUnlockSemaphore = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _unlockCancellationTokenSource = new CancellationTokenSource();
        private Timer _lockKeepaliveTimer;

        private readonly TimeSpan _expiryTime;
        private readonly TimeSpan? _waitTime;
        private readonly TimeSpan? _retryDelay;
        private CancellationToken _cancellationToken;

        private readonly TimeSpan _minimumExpiryTime = TimeSpan.FromMilliseconds(10);
        private readonly TimeSpan _minimumRetryTime = TimeSpan.FromMilliseconds(10);

        private readonly IDatabase _redisDatabase;
        private readonly ILogger<RedisLock> _logger;

        readonly RedisKey _key;
        readonly string _token;
        readonly int _acquireRetryCount;
        readonly int _acquireRetryDelayMs;
        private bool _isDisposed;

        public bool IsAcquired;
        public int ExtendCount { get; private set; }

        public RedisLock(IDatabase redisDatabase,
            ILogger<RedisLock> logger,
            RedisKey key,
            TimeSpan expiryTime,
            TimeSpan? waitTime,
            TimeSpan? retryTime,
            CancellationToken cancellationToken = default)
        {
            _logger = logger;

            if (expiryTime < _minimumExpiryTime)
            {
                logger.LogWarning($"Expiry time {expiryTime.TotalMilliseconds}ms too low, setting to {_minimumExpiryTime.TotalMilliseconds}ms");
                expiryTime = _minimumExpiryTime;
            }

            if (retryTime != null && retryTime.Value < _minimumRetryTime)
            {
                logger.LogWarning($"Retry time {retryTime.Value.TotalMilliseconds}ms too low, setting to {_minimumRetryTime.TotalMilliseconds}ms");
                retryTime = _minimumRetryTime;
            }

            _redisDatabase = redisDatabase;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            _expiryTime = expiryTime;
            _waitTime = waitTime;
            _retryDelay = retryTime;
            _key = key;
            _acquireRetryCount = 3;
            _acquireRetryDelayMs = 100;
            _token = Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
        }

        #region Sync

        public void Start()
        {
            if (_waitTime.HasValue && _retryDelay.HasValue && _waitTime.Value.TotalMilliseconds > 0 && _retryDelay.Value.TotalMilliseconds > 0)
            {
                var stopwatch = Stopwatch.StartNew();

                while (!IsAcquired && stopwatch.Elapsed <= _waitTime.Value)
                {
                    Acquire();

                    if (!IsAcquired)
                    {
                        TaskUtils.Delay(_retryDelay.Value, _cancellationToken).GetAwaiter().GetResult();
                    }
                }
            }
            else
            {
                Acquire();
            }

            _logger.LogInformation($"Lock status: {IsAcquired}, {_key} ({_token})");

            if (IsAcquired)
            {
                StartAutoExtendTimer();
            }
        }

        private void Acquire()
        {
            for (var i = 0; i < _acquireRetryCount; i++)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                var iteration = i + 1;
                _logger.LogDebug($"Lock attempt {iteration}/{_acquireRetryCount}: {_key} {_token}");

                IsAcquired = _redisDatabase.LockTake(_key.Value, _token, _expiryTime);

                _logger.LogDebug($"Acquired locks for {_key} {_token}, isAcquired: {IsAcquired}");

                if (IsAcquired)
                {
                    return;
                }

                // only sleep if we have more retries left
                if (i < _acquireRetryCount - 1)
                {
                    _logger.LogDebug($"Acquire {_key} sleeping {_acquireRetryDelayMs}ms");

                    TaskUtils.Delay(_acquireRetryDelayMs).GetAwaiter().GetResult();
                }
            }
        }

        private void Unlock()
        {
            _unlockCancellationTokenSource.Cancel();
            _extendUnlockSemaphore.Wait();
            try
            {
                _redisDatabase.LockRelease(_key.Value, _token);
            }
            finally
            {
                _extendUnlockSemaphore.Release();
            }
            IsAcquired = false;
        }
        #endregion

        #region Async

        public async Task StartAsync()
        {
            if (_waitTime.HasValue && _retryDelay.HasValue && _waitTime.Value.TotalMilliseconds > 0 && _retryDelay.Value.TotalMilliseconds > 0)
            {
                var stopwatch = Stopwatch.StartNew();

                // ReSharper disable PossibleInvalidOperationException
                while (!IsAcquired && stopwatch.Elapsed <= _waitTime.Value)
                {
                    await AcquireAsync().ConfigureAwait(false);

                    if (!IsAcquired)
                    {
                        await TaskUtils.Delay(_retryDelay.Value, _cancellationToken).ConfigureAwait(false);
                    }
                }
                // ReSharper restore PossibleInvalidOperationException
            }
            else
            {
                await AcquireAsync().ConfigureAwait(false);
            }

            _logger.LogInformation($"Lock status: {IsAcquired}, {_key} ({_token})");

            if (IsAcquired)
            {
                StartAutoExtendTimer();
            }
        }

        private async Task AcquireAsync()
        {
            for (var i = 0; i < _acquireRetryCount; i++)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                var iteration = i + 1;
                _logger.LogInformation($"Lock attempt {iteration}/{_acquireRetryCount}: {_key} {_token}");

                IsAcquired = await _redisDatabase.LockTakeAsync(_key.Value, _token, _expiryTime).ConfigureAwait(false);

                _logger.LogInformation($"Acquired locks for {_key.Value} {_token}, isAcquired: {IsAcquired}");

                if (IsAcquired)
                {
                    return;
                }

                // only sleep if we have more retries left
                if (i < _acquireRetryCount - 1)
                {
                    _logger.LogInformation($"Acquire {_key} sleeping {_acquireRetryDelayMs}ms");

                    await TaskUtils.Delay(_acquireRetryDelayMs).ConfigureAwait(false);
                }
            }
        }

        private async Task UnlockAsync()
        {
            _unlockCancellationTokenSource.Cancel();
            _extendUnlockSemaphore.Wait();
            try
            {
                await _redisDatabase.LockReleaseAsync(_key.Value, _token).ConfigureAwait(false);
            }
            finally
            {
                _extendUnlockSemaphore.Release();
            }
            IsAcquired = false;
        }

        #endregion

        #region KeepAlive

        private void ExtendLockLifetime()
        {
            try
            {
                var gotSemaphore = _extendUnlockSemaphore.Wait(0, _unlockCancellationTokenSource.Token);
                try
                {
                    if (!gotSemaphore)
                    {
                        // another extend operation is still running, so skip this one
                        _logger.LogWarning($"Lock renewal skipped due to another renewal still running: {_key} ({_token})");
                        return;
                    }

                    _logger.LogTrace($"Lock renewal timer fired: {_key} ({_token})");

                    Extend();
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Lock renewal timer thread failed: {_key}, {_token})", exception);
                }
                finally
                {
                    if (gotSemaphore)
                    {
                        _extendUnlockSemaphore.Release();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // unlock has been called, don't extend
            }
        }

        private void Extend()
        {
            try
            {
                _logger.LogTrace($"ExtendInstance enter {_key} {_token}");

                // Returns 1 on success, 0 on failure setting expiry or key not existing, -1 if the key value didn't match
                var result = _redisDatabase.LockExtend(_key.Value, _token, _expiryTime);

                IsAcquired = result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error extending lock instance {_key} {_token}, {ex.Message}, {ex.InnerException}, {ex.StackTrace}");

                IsAcquired = false;
            }

            _logger.LogTrace($"ExtendInstance exit {_key} {_token}");
        }

        private void StartAutoExtendTimer()
        {
            var interval = _expiryTime.TotalMilliseconds / 2;

            _logger.LogInformation($"Starting auto extend timer with {interval}ms interval");

            _lockKeepaliveTimer = new Timer(
                state => { ExtendLockLifetime(); },
                null,
                (int)interval,
                (int)interval);
        }

        private void StopKeepAliveTimer()
        {
            lock (_lockObject)
            {
                if (_lockKeepaliveTimer != null)
                {
                    _lockKeepaliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _lockKeepaliveTimer.Dispose();
                    _lockKeepaliveTimer = null;
                }
            }
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync(bool disposing)
        {
            _logger.LogInformation($"Disposing lock {_key} {_token}");

            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // managed resources
                StopKeepAliveTimer();
            }

            //unmanaged
            await UnlockAsync().ConfigureAwait(false);

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            _logger.LogInformation($"Disposing lock {_key} {_token}");

            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // managed resources
                StopKeepAliveTimer();
            }

            //unmanaged
            Unlock();

            _isDisposed = true;
        }

        ~RedisLock()
        {
            Dispose(false);
        }

        #endregion
    }
}
