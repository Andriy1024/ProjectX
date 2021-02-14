using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.MessageBus
{
    public class SubscribeProperties : IEventBusProperties
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public QueueOptions Queue { get; set; } = new QueueOptions();

        public ConsumerOptions Consumer { get; set; } = new ConsumerOptions();

        public override string ToString()
        {
            return $"{nameof(Exchange)}: {Exchange?.ToString()}, {nameof(Queue)}: {Queue?.ToString()}, {nameof(Consumer)}: {Consumer?.ToString()}.";
        }

        public static SubscribeProperties Validate(IEventBusProperties properties)
        {
            Utill.ThrowIfNull(properties, nameof(properties));

            if (!(properties is SubscribeProperties subscribeOptions))
                  throw new ArgumentException($"{nameof(properties)} should be {nameof(SubscribeProperties)} type.");

            ExchangeOptions.Validate(subscribeOptions.Exchange);


            return subscribeOptions;
        }
    }
}
