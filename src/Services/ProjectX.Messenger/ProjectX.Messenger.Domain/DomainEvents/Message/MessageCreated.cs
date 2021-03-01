using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageCreated : IDomainEvent
    {
        public Guid Id { get; private set; }

        public Guid ConversationId { get; private set; }

        public long AuthorId { get; private set; }

        public string Content { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MessageCreated() { }

        public MessageCreated(Guid id, Guid conversationId, long authorId, string content, DateTimeOffset createdAt)
        {
            Id = id;
            ConversationId = conversationId;
            AuthorId = authorId;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}
