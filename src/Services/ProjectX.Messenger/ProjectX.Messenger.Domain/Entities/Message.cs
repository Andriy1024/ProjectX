using System;

namespace ProjectX.Messenger.Domain
{
    /// <summary>
    /// Conversation message entity.
    /// </summary>
    public sealed class Message
    {
        public Guid Id { get; private set; }

        public ConversationId ConversationId { get; private set; }

        public long AuthorId { get; private set; }

        public string Content { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? UpdatedAt { get; private set; }

        private Message() { }

        public Message(ConversationId conversationId, Guid id, long authorId, string content, DateTimeOffset createdAt)
        {
            ConversationId = conversationId;
            Id = id;
            AuthorId = authorId;
            Content = content;
            CreatedAt = createdAt;
        }

        public void Update(string content, DateTimeOffset updatedAt) 
        {
            Content = content;
            UpdatedAt = updatedAt;
        }
    }
}
