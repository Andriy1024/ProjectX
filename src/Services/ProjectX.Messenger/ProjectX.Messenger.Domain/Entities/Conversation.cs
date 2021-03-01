using ProjectX.Core;
using ProjectX.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectX.Messenger.Domain
{
    /// <summary>
    /// Conversation aggregate.
    /// </summary>
    public sealed class Conversation : EventSourcedAggregate
    {
        public DateTimeOffset CreatedAt { get; private set; }

        public IReadOnlyCollection<Message> Messages => _messages;

        private readonly List<Message> _messages = new List<Message>();

        public IReadOnlyCollection<long> Members => _members;

        private readonly HashSet<long> _members = new HashSet<long>();

        private Conversation() { }

        public Conversation(Guid id, DateTimeOffset createdAt, IEnumerable<long> members, Message message) 
        {
            if(members.Count() <= 1) 
            {
                throw new InvalidDataException(ErrorCode.InvalidData); // to do specific code
            }

            var @event = new ConversationCreated(id, createdAt, members, message);
            Apply(@event);
        }

        public Conversation(IEnumerable<IDomainEvent> events) 
        {
            var conversation = new Conversation();
            conversation.Load(events);
        }

        public static Conversation Create(Guid id, DateTimeOffset createdAt, IEnumerable<long> members, Message message) 
        {
            return new Conversation(id, createdAt, members, message);
        }

        public void AddMessage(Guid id, long author, string content) 
        {
            var @event = new MessageCreated(id: id, 
                                            conversationId: Id, 
                                            authorId: author, 
                                            content: content, 
                                            createdAt: DateTimeOffset.UtcNow);

            Apply(@event);
        }

        public void UpdateMessage(Guid id, string content) 
        {
            Utill.ThrowIfNullOrEmpty(content, nameof(content));

            var message = GetMessageRequired(id);

            var @event = new MessageUpdated(conversationId: Id,
                                            messageId: message.Id,
                                            content: content,
                                            updatedAt: DateTimeOffset.UtcNow);

            Apply(@event);
        }

        public void DeleteMessage(Guid id) 
        {
            var message = GetMessageRequired(id);
            var @event = new MessageDeleted(conversationId: Id, messageId: message.Id);
            Apply(@event);
        }

        private Message GetMessageRequired(Guid id) 
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);

            if (message == null)
            {
                throw new NotFoundException(ErrorCode.NotFound); // to do add specific error code
            }

            return message;
        }

        private void When(ConversationCreated @event) 
        {
            Id = @event.Id;
            CreatedAt = @event.CreatedAt;
            _messages.Add(@event.Message);
        }

        private void When(MessageCreated @event) 
        {
            var message = new Message(conversationId: @event.ConversationId, 
                                      id: @event.Id,
                                      authorId: @event.AuthorId,
                                      content: @event.Content,
                                      createdAt: @event.CreatedAt);

            _messages.Add(message);
        }

        private void When(MessageUpdated @event) 
        {
            var message = GetMessageRequired(@event.MessageId);
            message.Update(@event.Content, @event.UpdatedAt);
        }

        private void When(MessageDeleted @event) 
        {
            var message = GetMessageRequired(@event.MessageId);

            _messages.Remove(message);
        }
    }
}
