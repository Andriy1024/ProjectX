using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageCreated : IDomainEvent
    {
        public Guid MessageId { get; private set; }

        public string ConversationId { get; private set; }

        public long AuthorId { get; private set; }

        public long Recipient { get; private set; }

        public string Content { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MessageCreated() { }

        public MessageCreated(Guid messageId, 
            string conversationId, 
            long authorId,
            long recipient,
            string content, 
            DateTimeOffset createdAt)
        {
            MessageId = messageId;
            ConversationId = conversationId;
            AuthorId = authorId;
            Recipient = recipient;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}
