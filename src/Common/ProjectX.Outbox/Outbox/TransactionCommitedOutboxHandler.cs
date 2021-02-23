using MediatR;
using ProjectX.Core.DataAccess;
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

        public async Task Handle(TransactionCommitedEvent @event, CancellationToken cancellationToken)
        {
            await _outboxTransaction.OnTransactionCompletedAsync();
        }
    }
}
