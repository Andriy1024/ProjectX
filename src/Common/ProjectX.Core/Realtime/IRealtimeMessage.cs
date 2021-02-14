namespace ProjectX.Core.Realtime
{
    /// <summary>
    /// Client's realtime notification message.
    /// </summary>
    public interface IRealtimeMessage
    {
        /// <summary>
        /// Type of the realtime message for client.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Payload data of the message.
        /// </summary>
        object Payload { get; }
    }
}
