using ProjectX.Core;
using ProjectX.Identity.Domain;
using ProjectX.Outbox;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.DomainEventHandlers
{
    public class UserDeletedDomainEventHandler : IDomainEventHandler<UserDeletedDomainEvent>
    {
        readonly IOutboxManager _outBox;

        public UserDeletedDomainEventHandler(IOutboxManager outBox)
        {
            _outBox = outBox;
        }

        public async Task Handle(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
        }
    }
}
