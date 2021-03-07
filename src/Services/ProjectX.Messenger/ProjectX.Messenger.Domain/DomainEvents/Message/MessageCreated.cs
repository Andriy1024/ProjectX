using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageCreated : IDomainEvent
    {
        public Guid MessageId { get; private set; }

        public string ConversationId { get; private set; }

        public IEnumerable<long> Users { get; set; }

        public long AuthorId { get; private set; }

        public string Content { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MessageCreated() { }

        public MessageCreated(Guid messageId, 
            string conversationId,
            IEnumerable<long> users,
            long authorId,
            string content, 
            DateTimeOffset createdAt)
        {
            MessageId = messageId;
            ConversationId = conversationId;
            Users = users;
            AuthorId = authorId;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}
