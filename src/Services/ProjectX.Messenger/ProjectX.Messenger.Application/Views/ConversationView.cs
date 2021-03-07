using ProjectX.Messenger.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectX.Messenger.Application.Views
{
    public class ConversationView
    {
        public string Id { get; set; }

        public IEnumerable<long> Users { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<MessageView> Messages { get; set; } = new List<MessageView>();

        public void Apply(ConversationStarted @event)
        {
            Id = @event.Id;
            Users = @event.Users;
            CreatedAt = @event.CreatedAt;
        }

        public void Apply(MessageCreated @event) => Messages.Add(new MessageView(id: @event.MessageId, 
                                                                                 conversationId: @event.ConversationId,
                                                                                 authorId: @event.AuthorId,
                                                                                 content: @event.Content,
                                                                                 createdAt: @event.CreatedAt));
        
        public void Apply(MessageDeleted @event) 
        {
            var message = Messages.FirstOrDefault(m => m.Id == @event.MessageId);

            if(message != null) Messages.Remove(message);
        }

        public void Apply(MessageUpdated @event) => Messages.FirstOrDefault(m => m.Id == @event.MessageId)?.Update(@event.Content, @event.UpdatedAt);
        
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
}
