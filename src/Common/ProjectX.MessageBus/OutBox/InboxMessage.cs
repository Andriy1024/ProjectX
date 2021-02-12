using System;

namespace ProjectX.MessageBus.OutBox
{
    public class InboxMessage
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public string MessageType { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
