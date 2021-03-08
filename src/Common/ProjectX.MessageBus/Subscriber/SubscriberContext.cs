using ProjectX.Core;
using RabbitMQ.Client;
using System;
using ProjectX.RabbitMq.Pipeline;

namespace ProjectX.RabbitMq
{
    internal sealed class SubscriberContext
    {
        public SubscriberContext(SubscriptionKey key,
                          Type eventType, 
                          IModel channel, 
                          QueueProperties queue, 
                          ExchangeProperties exchange, 
                          ConsumerProperties consumer,
                          Pipe.Handler<SubscriberRequest> handler)
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
            Handler = handler;
        }

        public SubscriptionKey Key { get; }

        public Type EventType { get; }

        public IModel Channel { get; }

        public QueueProperties Queue { get; }

        public ExchangeProperties Exchange { get; }

        public ConsumerProperties Consumer { get; }
          
        public Pipe.Handler<SubscriberRequest> Handler { get; }

        public override string ToString()
        {
            return $"{nameof(Queue)}: {Queue}, {nameof(Exchange)}: {Exchange}, {nameof(Consumer)}: {Consumer}, {nameof(EventType)}: {EventType.Name}";
        }
    }
}
