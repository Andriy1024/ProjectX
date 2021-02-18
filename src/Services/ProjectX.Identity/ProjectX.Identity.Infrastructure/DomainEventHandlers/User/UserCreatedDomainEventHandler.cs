using ProjectX.Contracts.IntegrationEvents;
using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Identity.Domain;
using ProjectX.MessageBus;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.DomainEventHandlers
{
    public sealed class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
    {
        private readonly IIntegrationEventService _integrationEvents;

        public UserCreatedDomainEventHandler(IIntegrationEventService integrationEvents)
        {
            _integrationEvents = integrationEvents;
        }

        public async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = domainEvent.User;
            _integrationEvents.Add(new UserCreatedIntegrationEvent(userId: user.Id, 
                                                                   firstName: user.FirstName,
                                                                   lastName: user.LastName,
                                                                   email: user.Email), 
                                   new PublishProperties(MessageBusExchanges.Identity));
        }
    }
}
