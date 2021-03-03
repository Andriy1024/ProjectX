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
    public sealed class Conversation : EventSourcedAggregate<ConversationId>
    {
        public ConversationId Id { get; private set; }

        public long FirstParticipant { get; private set; }

        public long SecondParticipant { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public IReadOnlyCollection<Message> Messages => _messages;

        private readonly List<Message> _messages = new List<Message>();

        public Conversation() { }

        private Conversation(ConversationId id, long firstParticipant, long secondParticipant) 
        {
            var @event = new ConversationStarted(id, DateTimeOffset.UtcNow, firstParticipant, secondParticipant);
            
            Apply(@event);
        }

        public Conversation(IEnumerable<IDomainEvent> events) 
        {
            var conversation = new Conversation();
            conversation.Load(events);
        }

        public static Conversation Start(long firstParticipant, long secondParticipant) 
        {
            return new Conversation(new ConversationId(firstParticipant, secondParticipant), firstParticipant, secondParticipant);
        }

        public override string GetId() => Id;

        public void AddMessage(Guid messageId, long author, string content) 
        {
            if(!IsBelongToConversation(author)) 
            {
                throw new InvalidPermissionException(ErrorCode.InvalidPermission, "The author doesn't belong to the conversation.");
            }

            var @event = new MessageCreated(messageId: messageId, 
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

        public void ClearMessageHistoryFor(long participant) 
        {
            if (!IsBelongToConversation(participant))
            {
                throw new InvalidPermissionException(ErrorCode.InvalidPermission, "The author doesn't belong to the conversation.");
            }

            var @event = new ParticipantHistoryCleared(Id, participant);
            Apply(@event);
        }

        private bool IsBelongToConversation(long userId) => FirstParticipant == userId || SecondParticipant == userId;
        
        private Message GetMessageRequired(Guid id) 
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);

            if (message == null)
            {
                throw new NotFoundException(ErrorCode.NotFound); // to do add specific error code
            }

            return message;
        }

        private void When(ConversationStarted @event) 
        {
            Id = new ConversationId(@event.FirstParticipant, @event.SecondParticipant);
            FirstParticipant = @event.FirstParticipant;
            SecondParticipant = @event.SecondParticipant;
            CreatedAt = @event.CreatedAt;
        }

        private void When(MessageCreated @event) 
        {
            var message = new Message(conversationId: Id, 
                                      id: @event.MessageId,
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

        private void When(ParticipantHistoryCleared @event) 
        {
        }
    }
}
