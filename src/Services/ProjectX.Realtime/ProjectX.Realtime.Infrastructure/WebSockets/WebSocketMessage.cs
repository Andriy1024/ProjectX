namespace ProjectX.Realtime.Infrastructure
{
    /// <summary>
    /// Represents a message received from client side.
    /// </summary>
    public sealed class WebSocketMessage
    {
        public WebSocketMessage(WebSocketContext connection, byte[] payload)
        {
            Connection = connection;
            Payload = payload;
        }

        public WebSocketContext Connection { get; }
        public byte[] Payload { get; }
    }
}
