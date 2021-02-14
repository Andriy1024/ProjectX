﻿using ProjectX.Core.IntegrationEvents;
using ProjectX.MessageBus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.IntegrationEvents
{
    public sealed class IntegrationEventService : IIntegrationEventService
    {
        private struct IntegrationEventWithProperties
        {
            public IIntegrationEvent IntegrationEvent { get; }

            public IEventBusProperties EventBusProperties { get; }

            public IntegrationEventWithProperties(IIntegrationEvent integrationEvent, IEventBusProperties eventBusProperties)
            {
                IntegrationEvent = integrationEvent;
                EventBusProperties = eventBusProperties;
            }
        }

        readonly Queue<IntegrationEventWithProperties> _integrationEvents = new Queue<IntegrationEventWithProperties>();

        readonly IMessageBus _eventBus;

        public IntegrationEventService(IMessageBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Add(IIntegrationEvent integrationEvent, IEventBusProperties eventBusProperties)
        {
            _integrationEvents.Enqueue(new IntegrationEventWithProperties(integrationEvent, eventBusProperties));
        }

        public Task PublishEventsThroughEventBusAsync()
        {
            while (_integrationEvents.TryDequeue(out IntegrationEventWithProperties eventWithProperties))
                   _eventBus.Publish(eventWithProperties.IntegrationEvent, eventWithProperties.EventBusProperties);

            return Task.CompletedTask;
        }
    }
}
