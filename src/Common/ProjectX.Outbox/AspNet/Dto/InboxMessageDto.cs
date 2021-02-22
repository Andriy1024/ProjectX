using System;

namespace ProjectX.Outbox.AspNet
{
    public class InboxMessageDto
    {
        public Guid Id { get; set; }

        public string MessageType { get; set; }

        public DateTime ProcessedAt { get; set; }

        public InboxMessageDto()
        {
        }

        public InboxMessageDto(Guid id, string messageType, DateTime processedAt)
        {
            Id = id;
            MessageType = messageType;
            ProcessedAt = processedAt;
        }
    }
}
