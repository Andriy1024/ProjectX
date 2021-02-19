using System;

namespace ProjectX.MessageBus.Outbox
{
    /// <summary>
    /// Using to avoid handling the same message multiple times.
    /// </summary>
    public class InboxMessage
    {
        public Guid Id { get; set; }
        public string MessageType { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
