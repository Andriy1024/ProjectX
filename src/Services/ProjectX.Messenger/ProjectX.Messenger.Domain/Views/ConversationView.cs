using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectX.Messenger.Domain
{
    public sealed class ConversationView
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

        public void Apply(MessageCreated @event) 
        {
            Messages.Add(new MessageView(id: @event.MessageId,
                             conversationId: @event.ConversationId,
                             authorId: @event.AuthorId,
                             content: @event.Content,
                             createdAt: @event.CreatedAt));
        }

        public void Apply(MessageDeleted @event)
        {
            var message = Messages.FirstOrDefault(m => m.Id == @event.MessageId);

            if (message != null) Messages.Remove(message);
        }

        public void Apply(MessageUpdated @event) => Messages.First(m => m.Id == @event.MessageId).Update(@event.Content, @event.UpdatedAt);
    }
}
