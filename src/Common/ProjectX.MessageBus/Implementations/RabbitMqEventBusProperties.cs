using ProjectX.Core.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectX.MessageBus.Implementations
{
    public sealed class RabbitMqEventBusProperties : IEventBusProperties
    {
        public EventBusExchanges Exchange { get; set; }

        public string RoutingKey { get; set; }

        public bool IsDirectExchange { get; set; }

        public bool IsBinary { get; set; }

        public RabbitMqEventBusProperties(EventBusExchanges exchange, bool isDirect, string routingKey = "", bool isBinary = false)
        {
            if (string.IsNullOrEmpty(exchange.Value))
                throw new ArgumentException($"Parameter {nameof(exchange)} can't be null or empty.");

            if (isDirect && string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Parameter {nameof(routingKey)} can't be null or empty for Direct exchange type.");

            Exchange = exchange;
            IsDirectExchange = isDirect;
            RoutingKey = routingKey;
            IsBinary = isBinary;
        }
    }
}
