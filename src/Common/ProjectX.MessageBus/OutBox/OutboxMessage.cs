using System;

namespace ProjectX.MessageBus.OutBox
{
    public class OutboxMessage
    {
        public string Id { get; set; }

        public string CorrelationId { get; set; }

        /// <summary>
        /// CLR type
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// JSON
        /// </summary>
        public string SerializedMessage { get; set; }

        public DateTime SentAt { get; set; }
    }
}
