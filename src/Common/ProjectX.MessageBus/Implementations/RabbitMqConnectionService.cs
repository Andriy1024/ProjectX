using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Threading;

namespace ProjectX.MessageBus.Implementations
{
    public sealed class RabbitMqConnectionService : IRabbitMqConnectionService
    {
        #region Private members

        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqConnectionService> _logger;
        private IConnection _connection;

        private bool _isDisposed;

        readonly ReaderWriterLockSlim _syncRoot = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        #endregion

        public RabbitMqConnectionService(IOptions<RabbitMqConnectionOptions> rabbitMqConnectionOptions, ILogger<RabbitMqConnectionService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (rabbitMqConnectionOptions == null || rabbitMqConnectionOptions.Value == null)
                throw new ArgumentNullException(nameof(rabbitMqConnectionOptions));

            _connectionFactory = new ConnectionFactory
            {
                UserName = rabbitMqConnectionOptions.Value.UserName,
                Password = rabbitMqConnectionOptions.Value.Password,
                VirtualHost = rabbitMqConnectionOptions.Value.VirtualHost,
                HostName = rabbitMqConnectionOptions.Value.HostName,
                Port = Convert.ToInt32(rabbitMqConnectionOptions.Value.Port),
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };
        }

        #region IRabbitMqConnectionService members

        public bool IsConnected
        {
            get
            {
                _syncRoot.EnterReadLock();
                try
                {
                    return _connection != null && _connection.IsOpen && !_isDisposed;
                }
                finally
                {
                    _syncRoot.ExitReadLock();
                }
            }
        }

        public IModel CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

            _syncRoot.EnterReadLock();
            try
            {
                return _connection.CreateModel();
            }
            finally
            {
                _syncRoot.ExitReadLock();
            }
        }

        public bool TryConnect()
        {
            if (IsConnected)
                return true;

            var connected = false;

            _logger.LogInformation("RabbitMQ Client is trying to connect");

            _syncRoot.EnterWriteLock();
            try
            {
                _connection = _connectionFactory.CreateConnection();

                if (_connection != null && _connection.IsOpen)
                {
                    //_connection.RecoverySucceeded += OnConnectionRecoverySucceeded;
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.ConnectionUnblocked += OnConnectionUnblocked;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    //_connection.ConnectionRecoveryError += OnConnectionRecoveryError;

                    connected = true;
                }
                else
                {
                    connected = false;
                }
            }
            finally
            {
                _syncRoot.ExitWriteLock();
            }

            if (connected)
                _logger.LogInformation($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
            else
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

            return connected;
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            _syncRoot.EnterWriteLock();
            try
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;

                try
                {
                    _connection?.Dispose();
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            finally
            {
                _syncRoot.ExitWriteLock();
            }
        }

        #endregion

        #region Private methods

        private bool IsDisposed()
        {
            _syncRoot.EnterReadLock();
            try
            {
                return _isDisposed;
            }
            finally
            {
                _syncRoot.ExitReadLock();
            }
        }

        #endregion

        #region Event handlers

        private void OnConnectionRecoverySucceeded(object sender, EventArgs e)
        {
            if (IsDisposed()) return;

            _logger.LogInformation($"A RabbitMQ connection recovery succeded.");
        }

        private void OnConnectionUnblocked(object sender, EventArgs e)
        {
            if (IsDisposed()) return;

            _logger.LogError($"A RabbitMQ connection unblocked.");
        }

        private void OnConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
            if (IsDisposed()) return;

            if (e.Exception != null)
            {
                _logger.LogError(e.Exception, $"A RabbitMQ connection recovery error. {e.Exception.Message} Trying to re-connect...");

                TryConnect();
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (IsDisposed()) return;

            _logger.LogError($"A RabbitMQ connection is blocked. Reason: {e.Reason} Trying to re-connect...");

            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (IsDisposed()) return;

            if (e.Exception != null)
            {
                _logger.LogError(e.Exception, $"A RabbitMQ connection throw exception. {e.Exception.Message}. Trying to re-connect...");

                TryConnect();
            }
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (IsDisposed()) return;

            if (reason != null)
            {
                _logger.LogError($"A RabbitMQ connection is on shutdown. {reason.ReplyText}. Trying to re-connect...");

                TryConnect();
            }
        }

        #endregion
    }
}
