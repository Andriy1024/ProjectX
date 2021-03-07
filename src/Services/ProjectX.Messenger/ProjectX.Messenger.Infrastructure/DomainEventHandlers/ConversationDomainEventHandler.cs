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
          IDomainEventHandler<MessageUpdated>
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

            _realtime.Add(realTimeMessege, new long[] {domainEvent.AuthorId, domainEvent.Recipient });

            return Task.CompletedTask;
        }

        public Task Handle(MessageDeleted domainEvent, CancellationToken cancellationToken)
        {
            //var realTimeMessege = new RealtimeMessageContext(
            //                          new MessageDeletedMessage(
            //                              messageId: domainEvent.MessageId,
            //                              conversationId: domainEvent.ConversationId));

            //_realtime.Add(realTimeMessege, new long[] { domainEvent.AuthorId, domainEvent.Recipient });

            return Task.CompletedTask;
        }

        public Task Handle(MessageUpdated domainEvent, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
