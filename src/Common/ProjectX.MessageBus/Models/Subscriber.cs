using ProjectX.Core;
using RabbitMQ.Client;
using System;

namespace ProjectX.MessageBus
{
    public class Subscriber
    {
        public Subscriber(SubscriptionKey key,
                          Type eventType, 
                          IModel channel, 
                          QueueOptions queue, 
                          ExchangeOptions exchange, 
                          ConsumerOptions consumer)
        {
            Utill.ThrowIfNull(eventType, nameof(eventType));
            Utill.ThrowIfNull(channel, nameof(channel));
            Utill.ThrowIfNull(queue, nameof(queue));
            Utill.ThrowIfNull(exchange, nameof(exchange));
            Utill.ThrowIfNull(consumer, nameof(consumer));

            Key = key;
            EventType = eventType;
            Channel = channel;
            Queue = queue;
            Exchange = exchange;
            Consumer = consumer;
        }

        public SubscriptionKey Key { get; }

        public Type EventType { get; }

        public IModel Channel { get; }

        public QueueOptions Queue { get; }

        public ExchangeOptions Exchange { get; }

        public ConsumerOptions Consumer { get; }

        public override string ToString()
        {
            return $"{nameof(Queue)}: {Queue}, {nameof(Exchange)}: {Exchange}, {nameof(Consumer)}: {Consumer}, {nameof(EventType)}: {EventType.Name}";
        }
    }
}
