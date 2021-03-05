using Microsoft.Extensions.Logging;
using ProjectX.Core;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure
{
    /// <summary>
    /// Represents the holder of the <see cref="WebSocket"/>.
    /// </summary>
    public sealed class WebSocketConnection : IDisposable
    {
        #region Private members

        private const int _receivePayloadBufferSize = 1024 * 4; // 4KB
        private bool _isDisposed;
        private readonly WebSocket _webSocket;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger<WebSocketConnection> _logger;
        private readonly ChannelQueue<byte[]> _senderChannel;
        private readonly IWebSocketHandler _handler;

        #endregion

        #region Constructor

        public WebSocketConnection(string connectionId,
            long userId,
            WebSocket webSocket,
            CancellationToken cancellationToken,
            ILoggerFactory loggerFactory,
            IWebSocketHandler handler)
        {
            Utill.ThrowIfNullOrEmpty(connectionId, nameof(connectionId));
            Utill.ThrowIfNull(handler, nameof(handler));
            Utill.ThrowIfNull(webSocket, nameof(webSocket));

            ConnectionId = connectionId;
            UserId = userId;
            _webSocket = webSocket;
            _cancellationToken = cancellationToken;
            _logger = loggerFactory.CreateLogger<WebSocketConnection>();
            _handler = handler;
            _senderChannel = new ChannelQueue<byte[]>(ProcessSendAsync, loggerFactory.CreateLogger<ChannelQueue<byte[]>>());
        }

        #endregion

        


        public string ConnectionId { get; }

        public long UserId { get; }

        public Task StartReceiveMessageAsync()
        {
            if (_isDisposed) return Task.CompletedTask;

            return ReceiveMessagesUntilCloseAsync();
        }

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                _senderChannel.Dispose();
                _webSocket.Dispose();
            }
        }

        #endregion

        #region IEquatable members

        public bool Equals(WebSocketConnection other)
        {
            if (other is null) return false;

            return ConnectionId.Equals(other.ConnectionId);
        }

        public override bool Equals(object other) => Equals(other as WebSocketConnection);

        public override int GetHashCode() => ConnectionId.GetHashCode();

        #endregion

        private WebSocketState GetWebSocketState()
        {
            try
            {
                return _webSocket.State;
            }
            catch (ObjectDisposedException)
            {
                return WebSocketState.Closed;
            }
        }

        private async Task ProcessSendAsync(byte[] message)
        {
            if (GetWebSocketState() == WebSocketState.Open && !_isDisposed)
            {
                try
                {
                    await _webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, _cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    
                    _logger.LogInformation($"Send operation was canceled. Session {ConnectionId}.");

                    //await DisconnectAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while sending message to session {ConnectionId}. Error: {ex.Message}");
                }
            }
        }

        private async Task ReceiveMessagesUntilCloseAsync()
        {
            try
            {
                // Do a 0 byte read so that idle connections don't allocate a buffer when waiting for a read
                var result = await _webSocket.ReceiveAsync(Memory<byte>.Empty, CancellationToken.None).ConfigureAwait(false);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    return;
                }

                byte[] buffer = new byte[_receivePayloadBufferSize];

                WebSocketReceiveResult webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false); ;

                while (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
                {
                    byte[] webSocketMessage = await ReceiveMessagePayloadAsync(webSocketReceiveResult, buffer).ConfigureAwait(false); ;

                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        await _handler.HandleMessageAsync(new WebSocketMessage(this, webSocketMessage));
                    }

                    webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false); ;
                }

                await DisconnectAsync(webSocketReceiveResult.CloseStatus.Value, webSocketReceiveResult.CloseStatusDescription).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is WebSocketException wsException)
                {
                    _logger.LogError(ex, $"WebSocketException occured. WebSocketError: {wsException.WebSocketErrorCode}. Message: {wsException.Message}");
                }
                else 
                {
                    _logger.LogError(ex, ex.Message);
                }

                await DisconnectAsync(WebSocketCloseStatus.InternalServerError).ConfigureAwait(false);
            }
        }

        private static byte[] BufferSliceToByteArray(byte[] buffer, int count)
        {
            byte[] newArray = new byte[count];
            Buffer.BlockCopy(buffer, 0, newArray, 0, count);
            return newArray;
        }

        private async Task<byte[]> ReceiveMessagePayloadAsync(WebSocketReceiveResult webSocketReceiveResult, byte[] receivePayloadBuffer)
        {
            byte[] messagePayload = null;

            if (webSocketReceiveResult.EndOfMessage)
            {
                messagePayload = new byte[webSocketReceiveResult.Count];
                Buffer.BlockCopy(receivePayloadBuffer, 0, messagePayload, 0, webSocketReceiveResult.Count);
                //Array.Copy(receivePayloadBuffer, messagePayload, webSocketReceiveResult.Count);
            }
            else
            {
                using (MemoryStream messagePayloadStream = new MemoryStream())
                {
                    messagePayloadStream.Write(receivePayloadBuffer, 0, webSocketReceiveResult.Count);

                    while (!webSocketReceiveResult.EndOfMessage)
                    {
                        webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None).ConfigureAwait(false);
                        
                        messagePayloadStream.Write(receivePayloadBuffer, 0, webSocketReceiveResult.Count);
                    }

                    messagePayload = messagePayloadStream.ToArray();
                }
            }

            return messagePayload;
        }

        private async Task DisconnectAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string statusDescription = "Normal closure.")
        {
            try
            {
                if (CanSend()) 
                {
                    await _webSocket.CloseOutputAsync(closeStatus, statusDescription, CancellationToken.None).ConfigureAwait(false);   
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Safe disconnect failed for session {ConnectionId}. Reason: {ex.Message}");
            }
            finally
            {
                Dispose();
            }

            await _handler.HandleDisconnectionAsync(this).ConfigureAwait(false);
        }

        public bool CanSend() 
        {
            var state = GetWebSocketState();
            return !(state == WebSocketState.Aborted || state == WebSocketState.Closed || state == WebSocketState.CloseSent);
        } 

        public async ValueTask SendAsync(byte[] message)
        {
            if (_isDisposed) return;

            await _senderChannel.EnqueueAsync(message);
        }
    }
}
