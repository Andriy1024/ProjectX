using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core.JSON;
using ProjectX.Realtime.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace ProjectX.Realtime.Infrastructure
{
    public sealed class WebSocketConnectionManager : IWebSocketMessageHandler
    {
        #region Private members

        private readonly IMemoryCache _tempSessionIdCache;
        private readonly ILogger<WebSocketConnectionManager> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RealtimeAppOptions _realtimeOptions;
        //private readonly IRoomManager _roomManager;
        private readonly ISystemTextJsonSerializer _serializer;

        #endregion

        #region Constructor

        public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger,
            ILoggerFactory loggerFactory,
            IOptions<RealtimeAppOptions> realtimeOptions,
            //IRoomManager roomManager,
            ISystemTextJsonSerializer serializer)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            //_roomManager = roomManager;
            _realtimeOptions = realtimeOptions.Value;
            _serializer = serializer;

            var cacheOptions = new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromSeconds(_realtimeOptions.ExpirationScanFrequency) };
            _tempSessionIdCache = new MemoryCache(cacheOptions, _loggerFactory);
        }

        #endregion

        #region IWebSocketConnectionManager members

        public string GenerateConnectionId(long userId)
        {
            var connectionId = Guid.NewGuid().ToString();

            _tempSessionIdCache.Set(connectionId, userId, TimeSpan.FromSeconds(_realtimeOptions.CacheItemExpirationTime));

            return connectionId;
        }

        public bool TryGetUserId(string connectionId, out long userId)
        {
            var exist = _tempSessionIdCache.TryGetValue(connectionId, out userId);

            if (exist)
                _tempSessionIdCache.Remove(connectionId);

            return exist;
        }

        public async Task ConnectAsync(string connectionId, long userId, WebSocket webSocket, CancellationToken cancellationToken)
        {
            var webSocketConnection = new WebSocketConnection(connectionId, userId, webSocket, cancellationToken, _loggerFactory, this);

            _roomManager.AddConnection(webSocketConnection);
            _logger.LogInformation($"New WebSocket session {webSocketConnection.ConnectionId} connected.");

            await webSocketConnection.StartReceiveMessageAsync().ConfigureAwait(false);
        }

        #endregion

        #region IWebSocketHandler members

        public void HandleDisconnection(WebSocketConnection connection)
        {
            //_roomManager.RemoveConnection(connection);
        }

        public void HandleMessage(WebSocketMessage message)
        {
            //var response = new ClientMessageResponse();
            //try
            //{
            //    _logger.LogInformation($"Start handle socket client message.");
            //    var subscribeMessage = ConvertBytesToMessage(message.Payload);

            //    if (subscribeMessage == default)
            //    {
            //        response.Success = false;
            //        response.Message = "Invalid request model";
            //        return;
            //    }

            //    response.RequestId = subscribeMessage.RequestId;

            //    if (subscribeMessage.Action == SubscribeAction.Subscribe)
            //    {
            //        _roomManager.JoinRoom(message.Connection, subscribeMessage);
            //        response.Success = true;
            //        response.Message = "Subscription was successful";
            //    }
            //    else if (subscribeMessage.Action == SubscribeAction.Unsubscribe)
            //    {
            //        _roomManager.LeaveRoom(message.Connection, subscribeMessage);
            //        response.Success = true;
            //        response.Message = "Unsubscription was successful";
            //    }
            //    else
            //    {
            //        response.Success = false;
            //        response.Message = $"Invalid subscribe action: {subscribeMessage.Action}";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, ex.Message);
            //    response.Success = false;
            //    response.Message = ex.Message;
            //}
            //finally
            //{
            //    var bytes = ConvertMessageToBytes(response);
            //    message.Connection.Send(bytes);
            //}
        }

        #endregion

        #region Private methods

        private byte[] ConvertMessageToBytes<T>(T message)
        {
            return _serializer.SerializeToBytes(message);
        }

        //private ClientMessageRequest ConvertBytesToMessage(byte[] messageBytes)
        //{
        //    try
        //    {
        //        var request = _serializer.Deserialize<ClientMessageRequest>(messageBytes);
        //        if (request == null)
        //            throw new InvalidDataException(ErrorCode.InvalidData);

        //        return request;
        //    }
        //    catch (JsonException ex)
        //    {
        //        return null;
        //    }
        //}

        #endregion
    }
}
