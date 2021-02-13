using ProjectX.Core.IntegrationEvents;

namespace ProjectX.MessageBus
{
    public interface IMessageBus
    {
        void Subscribe<T>(SubscribeOptions properties)
            where T : IIntegrationEvent;

        void Unsubscribe<T>(SubscribeOptions properties)
            where T : IIntegrationEvent;

        void Publish<T>(T integrationEvent, PublishOptions properties)
            where T : IIntegrationEvent;

        void AddPublisher(PublishOptions properties);

        bool RemovePublisher(PublishOptions properties);
    }
}
