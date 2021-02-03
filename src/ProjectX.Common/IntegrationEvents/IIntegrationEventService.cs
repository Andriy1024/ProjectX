using System.Threading.Tasks;

namespace ProjectX.Common.IntegrationEvents
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync();

        void Add(IIntegrationEvent integrationEvent, IEventBusProperties eventBusProperties);
    }


    public interface IEventBusProperties { }
}
