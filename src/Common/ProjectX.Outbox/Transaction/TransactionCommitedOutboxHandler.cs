using MediatR;
using ProjectX.DataAccess;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class TransactionCommitedOutboxHandler : INotificationHandler<TransactionCommitedEvent>
    {
        private readonly IOutboxTransaction _outboxTransaction;

        public TransactionCommitedOutboxHandler(IOutboxTransaction outboxTransaction)
        {
            _outboxTransaction = outboxTransaction;
        }

        public Task Handle(TransactionCommitedEvent @event, CancellationToken cancellationToken)
        {
            return _outboxTransaction.OnTransactionCommitedAsync();
        }
    }
}
