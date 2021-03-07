using ProjectX.DataAccess;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class TransactionCommitedOutboxHandler : ITransactionCommitedEventHandler
    {
        private readonly IOutboxTransactionContext _outboxTransaction;

        public TransactionCommitedOutboxHandler(IOutboxTransactionContext outboxTransaction)
        {
            _outboxTransaction = outboxTransaction;
        }

        public Task Handle(TransactionCommitedEvent @event, CancellationToken cancellationToken)
        {
            return _outboxTransaction.OnTransactionCommitedAsync();
        }
    }
}
