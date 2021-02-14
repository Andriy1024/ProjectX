using System.Threading.Tasks;

namespace ProjectX.Core.IntegrationEvents
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync();

        void Add(IIntegrationEvent integrationEvent, IEventBusProperties eventBusProperties);
    }
}
