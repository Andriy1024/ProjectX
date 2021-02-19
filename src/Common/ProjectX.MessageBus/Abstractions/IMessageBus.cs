using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.MessageBus
{
    public interface IMessageBus
    {
        void Subscribe<T>(SubscribeProperties properties)
            where T : IIntegrationEvent;

        void Subscribe<T>(Action<SubscribeProperties> properties)
            where T : IIntegrationEvent;

        void Unsubscribe<T>(SubscribeProperties properties)
            where T : IIntegrationEvent;

        void Publish<T>(T integrationEvent, PublishProperties properties)
            where T : IIntegrationEvent;

        void Publish<T>(T integrationEvent, Action<PublishProperties> properties)
          where T : IIntegrationEvent;

        void AddPublisher(PublishProperties properties);

        bool RemovePublisher(PublishProperties properties);
    }
}
