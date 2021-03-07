using ProjectX.Core;
using ProjectX.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectX.Messenger.Domain
{
    public sealed class ConversationAggregate : EventSourcedAggregate
    {
        public ConversationId Id { get; private set; }

        public long FirstParticipant { get; private set; }

        public long SecondParticipant { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public IReadOnlyCollection<MessageEntity> Messages => _messages;

        private readonly List<MessageEntity> _messages = new List<MessageEntity>();

        public ConversationAggregate() { }

        private ConversationAggregate(ConversationId id, long firstParticipant, long secondParticipant) 
        {
            var @event = new ConversationStarted(id, DateTimeOffset.UtcNow, firstParticipant, secondParticipant);
            
            Apply(@event);
        }

        public ConversationAggregate(IEnumerable<IDomainEvent> events) 
        {
            var conversation = new ConversationAggregate();
            
            conversation.Load(events);
        }

        public static ConversationAggregate Start(long firstParticipant, long secondParticipant) 
        {
            return new ConversationAggregate(new ConversationId(firstParticipant, secondParticipant), firstParticipant, secondParticipant);
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
                                            recipient: author == FirstParticipant ? FirstParticipant: SecondParticipant,
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

        public void DeleteMessage(Guid id, long deleteBy) 
        {
            var message = GetMessageRequired(id);
            
            if(message.AuthorId != deleteBy) 
            {
                throw new InvalidPermissionException(ErrorCode.InvalidPermission, "Message can be deleted only by the owner.");
            }

            var @event = new MessageDeleted(conversationId: Id, messageId: message.Id);
            
            Apply(@event);
        }

        private bool IsBelongToConversation(long userId) => FirstParticipant == userId || SecondParticipant == userId;
        
        private MessageEntity GetMessageRequired(Guid id) 
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);

            if (message == null)
            {
                throw new NotFoundException(ErrorCode.ConversationMessageNotFound); // to do add specific error code
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
            var message = new MessageEntity(conversationId: Id, 
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

        protected override void When(IDomainEvent @event) 
        {
            switch (@event)
            {
                case ConversationStarted e:
                    When(e);
                    break;
                case MessageCreated e:
                    When(e);
                    break;
                case MessageUpdated e:
                    When(e);
                    break;
                case MessageDeleted e:
                    When(e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown event type: {@event.GetType().Name}.");
            }
        }
    }
}
