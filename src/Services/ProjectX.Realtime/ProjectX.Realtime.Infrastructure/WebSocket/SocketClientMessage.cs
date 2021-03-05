namespace ProjectX.Realtime.Infrastructure
{
    /// <summary>
    /// Represents a message received from client side.
    /// </summary>
    public sealed class WebSocketMessage
    {
        public WebSocketMessage(WebSocketConnection connection, byte[] payload)
        {
            Connection = connection;
            Payload = payload;
        }

        public WebSocketConnection Connection { get; }
        public byte[] Payload { get; }
    }
}
