using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.RabbitMq
{
    public interface IRabbitMqSubscriber
    {
        void Subscribe<T>(SubscribeProperties properties)
            where T : IIntegrationEvent;

        void Subscribe<T>(Action<SubscribeProperties> properties)
            where T : IIntegrationEvent;

        void Unsubscribe<T>(SubscribeProperties properties)
            where T : IIntegrationEvent;
    }
}
