using MediatR;

namespace ProjectX.DataAccess
{
    public sealed class TransactionCommitedEvent : INotification
    {
    }

    public interface ITransactionCommitedEventHandler : INotificationHandler<TransactionCommitedEvent> 
    {
    }
}
