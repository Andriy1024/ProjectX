using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageCreated : IDomainEvent
    {
        public Guid MessageId { get; private set; }

        public string ConversationId { get; private set; }

        public long AuthorId { get; private set; }

        public string Content { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MessageCreated() { }

        public MessageCreated(Guid messageId, ConversationId conversationId, long authorId, string content, DateTimeOffset createdAt)
        {
            MessageId = messageId;
            ConversationId = conversationId;
            AuthorId = authorId;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}
