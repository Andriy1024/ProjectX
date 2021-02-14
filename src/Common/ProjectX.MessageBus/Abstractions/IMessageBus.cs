using ProjectX.Core.IntegrationEvents;

namespace ProjectX.MessageBus
{
    public interface IMessageBus
    {
        void Subscribe<T>(IEventBusProperties properties)
            where T : IIntegrationEvent;

        void Unsubscribe<T>(IEventBusProperties properties)
            where T : IIntegrationEvent;

        void Publish<T>(T integrationEvent, IEventBusProperties properties)
            where T : IIntegrationEvent;

        void AddPublisher(IEventBusProperties properties);

        bool RemovePublisher(IEventBusProperties properties);
    }
}
