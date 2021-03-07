using ProjectX.Contracts.IntegrationEvents;
using ProjectX.Core;
using ProjectX.Identity.Domain;
using ProjectX.Outbox;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.DomainEventHandlers
{
    public sealed class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
    {
        private readonly IOutboxTransactionContext _outbox;

        public UserCreatedDomainEventHandler(IOutboxTransactionContext integrationEvents)
        {
            _outbox = integrationEvents;
        }

        public async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = domainEvent.User;

            await _outbox.AddAsync(new UserCreatedIntegrationEvent(id: Guid.NewGuid(),
                                                                   userId: user.Id, 
                                                                   firstName: user.FirstName,
                                                                   lastName: user.LastName,
                                                                   email: user.Email));
        }
    }
}
