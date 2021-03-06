using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.Threading.Workers;
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
    public sealed class WebSocketContext : IDisposable
    {
        public delegate Task MessageHandler(WebSocketMessage message);

        private const int _receivePayloadBufferSize = 1024 * 4; // 4KB
        private bool _isDisposed;
        private readonly WebSocket _webSocket;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger<WebSocketContext> _logger;
        private readonly ChannelWorker<byte[]> _senderChannel;
        private readonly MessageHandler _messageHandler;

        public WebSocketContext(ConnectionId connectionId,
                                long userId,
                                WebSocket webSocket,
                                CancellationToken cancellationToken,
                                ILoggerFactory loggerFactory,
                                MessageHandler messageHandler)
        {
            Utill.ThrowIfNullOrEmpty(connectionId, nameof(connectionId));
            Utill.ThrowIfNull(messageHandler, nameof(messageHandler));
            Utill.ThrowIfNull(webSocket, nameof(webSocket));

            ConnectionId = connectionId;
            UserId = userId;
            _webSocket = webSocket;
            _cancellationToken = cancellationToken;
            _logger = loggerFactory.CreateLogger<WebSocketContext>();
            _messageHandler = messageHandler;
            _senderChannel = new ChannelWorker<byte[]>(ProcessSendAsync, loggerFactory.CreateLogger<ChannelWorker<byte[]>>());
        }

        public ConnectionId ConnectionId { get; }

        public long UserId { get; }

        public WebSocketState GetWebSocketState()
        {
            try
            {
                return _isDisposed ? WebSocketState.Closed : _webSocket.State;
            }
            catch (Exception)
            {
                return WebSocketState.Closed;
            }
        }

        public async ValueTask SendAsync(byte[] message)
        {
            if (_isDisposed) return;

            await _senderChannel.EnqueueAsync(message);
        }

        private async Task ProcessSendAsync(byte[] message)
        {
            if (GetWebSocketState() == WebSocketState.Open)
            {
                try
                {
                    await _webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, _cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Ignore aborts, don't treat them like transport errors
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while sending message to session {ConnectionId}. Error: {ex.Message}");
                }
            }
        }

        public Task StartReceiveMessageAsync()
        {
            if (_isDisposed) return Task.CompletedTask;

            return ReceiveMessagesUntilCloseAsync();
        }

        private async Task ReceiveMessagesUntilCloseAsync()
        {
            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;

            try
            {
                // Do a 0 byte read so that idle connections don't allocate a buffer when waiting for a read
                var check = await _webSocket.ReceiveAsync(Memory<byte>.Empty, CancellationToken.None).ConfigureAwait(false);

                if (check.MessageType == WebSocketMessageType.Close)
                {
                    return;
                }

                byte[] buffer = new byte[_receivePayloadBufferSize];

                WebSocketReceiveResult webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken).ConfigureAwait(false); ;

                while (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
                {
                    byte[] webSocketMessage = await ReceiveMessagePayloadAsync(webSocketReceiveResult, buffer).ConfigureAwait(false); ;

                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        await _messageHandler(new WebSocketMessage(this, webSocketMessage));
                    }
                    else 
                    {
                        _logger.LogInformation($"Unsupported message type {webSocketReceiveResult.MessageType}");
                    }

                    webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken).ConfigureAwait(false); ;
                }
            }
            catch(OperationCanceledException ex) when (ex.CancellationToken.Equals(_cancellationToken))
            {
                // Ignore aborts, don't treat them like transport errors
            }
            catch (WebSocketException wsException) 
            {
                if(wsException.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely) 
                {
                    _logger.LogInformation("Client has closed the WebSocket connection without completing the close handshake");
                    
                    closeStatus = WebSocketCloseStatus.EndpointUnavailable;
                }
                else 
                {
                    _logger.LogError(wsException, $"WebSocketException occured. WebSocketError: {wsException.WebSocketErrorCode}. Message: {wsException.Message}");
                    
                    closeStatus = WebSocketCloseStatus.InternalServerError;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                
                closeStatus = WebSocketCloseStatus.InternalServerError;
            }
            finally 
            {
                await DisconnectAsync(closeStatus).ConfigureAwait(false);
            }
        }

        private async Task<byte[]> ReceiveMessagePayloadAsync(WebSocketReceiveResult webSocketReceiveResult, byte[] receivePayloadBuffer)
        {
            if (webSocketReceiveResult.EndOfMessage)
            {
                var messagePayload = new byte[webSocketReceiveResult.Count];
                
                Buffer.BlockCopy(receivePayloadBuffer, 0, messagePayload, 0, webSocketReceiveResult.Count);
                
                //Array.Copy(receivePayloadBuffer, messagePayload, webSocketReceiveResult.Count);
                
                return messagePayload;
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

                    return messagePayloadStream.ToArray();
                }
            }
        }

        private async Task DisconnectAsync(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure)
        {
            try
            {
                if (CanClose()) 
                {
                    await _webSocket.CloseOutputAsync(closeStatus, "", CancellationToken.None).ConfigureAwait(false);   
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
        }

        private bool CanClose() 
        {
            var state = GetWebSocketState();

            return !(state == WebSocketState.Aborted || state == WebSocketState.Closed || state == WebSocketState.CloseSent);
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

        public bool Equals(WebSocketContext other)
        {
            if (ReferenceEquals(other, null)) return false;

            return ConnectionId.Equals(other.ConnectionId);
        }

        public override bool Equals(object other) => Equals(other as WebSocketContext);

        public override int GetHashCode() => ConnectionId.GetHashCode();

        #endregion
    }
}
