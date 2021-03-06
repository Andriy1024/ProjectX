using Microsoft.Extensions.Logging;
using ProjectX.Core.JSON;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ProjectX.Core.Threading;

namespace ProjectX.Realtime.Infrastructure
{
    public sealed class WebSocketConnectionManager 
    {
        private readonly ILogger<WebSocketConnectionManager> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISystemTextJsonSerializer _serializer;

        private readonly Dictionary<long, HashSet<WebSocketContext>> _userIdToConnections = new Dictionary<long, HashSet<WebSocketContext>>();
        private readonly ReaderWriterLockSlim _connectionsSync = new ReaderWriterLockSlim();

        public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger,
                                          ILoggerFactory loggerFactory,
                                          ISystemTextJsonSerializer serializer)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _serializer = serializer;
        }

        /// <summary>
        /// The action adds the WebSocket to the collection and begins receiving messages from the WebSocket.
        /// </summary>
        public async Task HandleAsync(ConnectionId connectionId, long userId, WebSocket webSocket, CancellationToken cancellationToken)
        {
            var webSocketContext = new WebSocketContext(connectionId, userId, webSocket, cancellationToken, _loggerFactory, HandleMessageAsync);
            
            bool added = false;

            using (new WriteLock(_connectionsSync)) 
            {
                if(_userIdToConnections.TryGetValue(userId, out var connections)) 
                {
                    added = connections.Add(webSocketContext);
                }
                else 
                {
                    _userIdToConnections.Add(userId, new HashSet<WebSocketContext>() { webSocketContext });
                }
            }

            if (!added) 
            {
                webSocketContext.Dispose();
                
                throw new Exception($"Connection failed WebSocketContext {connectionId}.");
            }

            _logger.LogInformation($"WebSocket was connected, id: {webSocketContext.ConnectionId}.");

            try
            {
                await webSocketContext.StartReceiveMessageAsync().ConfigureAwait(false);
            }
            finally 
            {
                await HandleDisconnectionAsync(webSocketContext).ConfigureAwait(false);
            }
        }

        public Task HandleDisconnectionAsync(WebSocketContext connection)
        {
            using (new WriteLock(_connectionsSync))
            {
                if (_userIdToConnections.TryGetValue(connection.UserId, out var connections))
                {
                    connections.Remove(connection);

                    if (connections.Count == 0)
                    {
                        _userIdToConnections.Remove(connection.UserId);
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Send the message to all active connections.
        /// </summary>
        public Task SendAsync<T>(T message) 
        {
            WebSocketContext[] receivers = null;

            using (new ReadLock(_connectionsSync)) 
            {
                receivers = _userIdToConnections.SelectMany(c => c.Value).ToArray();
            }

            return SendAsync(message, receivers);
        }

        /// <summary>
        /// Send the message to the users.
        /// </summary>
        public Task SendAsync<T>(T message, IEnumerable<long> users) 
        {
            WebSocketContext[] receivers = null;

            using (new ReadLock(_connectionsSync))
            {
                receivers = _userIdToConnections.Where(c => users.Contains(c.Key))
                                                .SelectMany(c => c.Value)
                                                .ToArray();
            }

            return SendAsync(message, receivers);
        }

        public WebSocketContext[] GetConnections() 
        {
            using (new ReadLock(_connectionsSync))
            {
                return _userIdToConnections.SelectMany(c => c.Value)
                                           .ToArray();
            }
        }

        private async Task SendAsync<T>(T message, WebSocketContext[] connections) 
        {
            if (connections.Length == 0) return;

            var byteMessage = _serializer.SerializeToBytes(message);

            for (int i = 0; i < connections.Length; i++)
            {
                try
                {
                    await connections[i].SendAsync(byteMessage);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Represents <see cref="WebSocketContext.MessageHandler">.
        /// Extend this in future if will be needed.
        /// </summary>
        public Task HandleMessageAsync(WebSocketMessage message)
        {
            try
            {
                Console.WriteLine(Encoding.UTF8.GetString(message.Payload));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
