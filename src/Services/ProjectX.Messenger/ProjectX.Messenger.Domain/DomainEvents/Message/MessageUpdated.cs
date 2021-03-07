using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageUpdated : IDomainEvent
    {
        public string ConversationId { get; set; }

        public IEnumerable<long> Users { get; set; }

        public Guid MessageId { get; set; }

        public string Content { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        private MessageUpdated() {}

        public MessageUpdated(string conversationId, IEnumerable<long> users, Guid messageId, string content, DateTimeOffset updatedAt)
        {
            ConversationId = conversationId;
            Users = users;
            MessageId = messageId;
            Content = content;
            UpdatedAt = updatedAt;
        }
    }
}
