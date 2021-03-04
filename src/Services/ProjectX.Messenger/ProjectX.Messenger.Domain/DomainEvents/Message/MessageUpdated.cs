using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageUpdated : IDomainEvent
    {
        public string ConversationId { get; set; }

        public Guid MessageId { get; set; }

        public string Content { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        private MessageUpdated() {}

        public MessageUpdated(string conversationId, Guid messageId, string content, DateTimeOffset updatedAt)
        {
            ConversationId = conversationId;
            MessageId = messageId;
            Content = content;
            UpdatedAt = updatedAt;
        }
    }
}
