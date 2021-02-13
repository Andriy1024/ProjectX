using ProjectX.Core.IntegrationEvents;

namespace ProjectX.MessageBus
{
    public interface IEventBus
    {
        void Subscribe<T, TH>(IEventBusProperties properties)
            where T : IIntegrationEvent;

        void Unsubscribe<T, TH>(IEventBusProperties properties)
            where T : IIntegrationEvent;

        void Publish(IIntegrationEvent integrationEvent, IEventBusProperties properties);
        void AddPublisher(IEventBusProperties properties);
        bool RemovePublisher(IEventBusProperties properties);
    }
}
