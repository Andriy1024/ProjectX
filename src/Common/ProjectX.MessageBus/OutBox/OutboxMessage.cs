using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.MessageBus.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }    

        /// <summary>
        /// CLR type
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// JSON
        /// </summary>
        public string SerializedMessage { get; set; }

        /// <summary>
        /// Not mapped
        /// </summary>
        public IIntegrationEvent Message { get; set; }

        /// <summary>
        /// Not mapped
        /// </summary>
        public Type Type { get; set; }

        public DateTime SavedAt { get; set; }

        public DateTime? SentAt { get; set; }

        public OutboxMessage()
        {
        }

        public OutboxMessage(IIntegrationEvent message, string serializedMessage, DateTime savedAt)
        {
            Id = message.Id;
            Message = message;
            Type = message.GetType();
            MessageType = Type.FullName;
            SerializedMessage = serializedMessage;
            SavedAt = savedAt;
        }
    }
}
