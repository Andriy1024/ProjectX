using System;

namespace ProjectX.Messenger.Domain
{
    public class MessageView
    {
        public Guid Id { get; set; }

        public string ConversationId { get; set; }

        public long AuthorId { get; set; }

        public string Content { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public MessageView()
        {
        }

        public MessageView(Guid id, string conversationId, long authorId, string content, DateTimeOffset createdAt)
        {
            Id = id;
            ConversationId = conversationId;
            AuthorId = authorId;
            Content = content;
            CreatedAt = createdAt;
        }

        public void Update(string content, DateTimeOffset updatedAt) => (Content, UpdatedAt) = (content, updatedAt);
    }
}
