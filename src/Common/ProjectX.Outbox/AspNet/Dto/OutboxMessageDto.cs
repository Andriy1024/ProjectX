using System;

namespace ProjectX.Outbox.AspNet
{
    public class OutboxMessageDto
    {
        public Guid Id { get; set; }

        public string MessageType { get; set; }

        public string SerializedMessage { get; set; }

        public DateTime SavedAt { get; set; }

        public DateTime? SentAt { get; set; }

        public OutboxMessageDto()
        {
        }

        public OutboxMessageDto(Guid id, string messageType, string serializedMessage, DateTime savedAt, DateTime? sentAt)
        {
            Id = id;
            MessageType = messageType;
            SerializedMessage = serializedMessage;
            SavedAt = savedAt;
            SentAt = sentAt;
        }
    }
}
