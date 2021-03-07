using ProjectX.Core;
using ProjectX.Messenger.Domain;
using ProjectX.Realtime;
using ProjectX.Realtime.Messenger;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Infrastructure.DomainEventHandlers
{
    public sealed class ConversationDomainEventHandler
        : IDomainEventHandler<MessageCreated>,
          IDomainEventHandler<MessageDeleted>,
          IDomainEventHandler<MessageUpdated>,
          IDomainEventHandler<ConversationStarted>
    {
        private readonly IRealtimeTransactionContext _realtime;

        public ConversationDomainEventHandler(IRealtimeTransactionContext realtime)
        {
            _realtime = realtime;
        }

        public Task Handle(MessageCreated domainEvent, CancellationToken cancellationToken)
        {
            var realTimeMessege = new RealtimeMessageContext(
                                      new MessageCreatedMessage(
                                          authorId: domainEvent.AuthorId,
                                          messageId: domainEvent.MessageId,
                                          conversationId: domainEvent.ConversationId,
                                          content: domainEvent.Content,
                                          createdAt: domainEvent.CreatedAt));

            _realtime.Add(realTimeMessege, domainEvent.Users);

            return Task.CompletedTask;
        }

        public Task Handle(MessageDeleted domainEvent, CancellationToken cancellationToken)
        {
            var realTimeMessege = new RealtimeMessageContext(
                                      new MessageDeletedMessage(
                                          messageId: domainEvent.MessageId,
                                          conversationId: domainEvent.ConversationId));

            _realtime.Add(realTimeMessege, domainEvent.Users);

            return Task.CompletedTask;
        }

        public Task Handle(MessageUpdated domainEvent, CancellationToken cancellationToken)
        {
            var realTimeMessege = new RealtimeMessageContext(
                                      new MessageUpdatedMessage(
                                          messageId: domainEvent.MessageId,
                                          conversationId: domainEvent.ConversationId,
                                          content: domainEvent.Content,
                                          updatedAt: domainEvent.UpdatedAt));

            _realtime.Add(realTimeMessege, domainEvent.Users);

            return Task.CompletedTask;
        }

        public Task Handle(ConversationStarted notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
