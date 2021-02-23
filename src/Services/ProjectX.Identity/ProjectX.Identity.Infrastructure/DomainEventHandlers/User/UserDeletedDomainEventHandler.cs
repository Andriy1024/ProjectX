using ProjectX.Contracts.IntegrationEvents;
using ProjectX.Core;
using ProjectX.Identity.Domain;
using ProjectX.Outbox;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.DomainEventHandlers
{
    public class UserDeletedDomainEventHandler : IDomainEventHandler<UserDeletedDomainEvent>
    {
        private readonly IOutboxTransaction _outBox;

        public UserDeletedDomainEventHandler(IOutboxTransaction outBox)
        {
            _outBox = outBox;
        }

        public async Task Handle(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await _outBox.AddAsync(new UserDeletedIntegrationEvent(Guid.NewGuid(), domainEvent.User.Id));
        }
    }
}
