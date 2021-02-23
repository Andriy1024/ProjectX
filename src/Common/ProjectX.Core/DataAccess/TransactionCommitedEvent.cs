using MediatR;

namespace ProjectX.Core.DataAccess
{
    public sealed class TransactionCommitedEvent : INotification
    {
        public TransactionCommitedEvent(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }
    }
}
