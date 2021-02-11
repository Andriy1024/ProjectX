using ProjectX.Core.IntegrationEvents;

namespace ProjectX.MessageBus
{
    public interface IEventBus
    {
        void Subscribe<T, TH>(IEventBusProperties properties, TH handler)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>(IEventBusProperties properties)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Publish(IIntegrationEvent integrationEvent, IEventBusProperties properties);
        void AddPublisher(IEventBusProperties properties);
        bool RemovePublisher(IEventBusProperties properties);
    }
}
