namespace ProjectX.MessageBus
{
    public class ExchangeOptions
    {
        public EventBusExchanges Name { get; set; }

        public ExchangeType Type { get; set; } = ExchangeType.Direct;

        public bool AutoDelete { get; set; } = true;

        public bool Durable { get; set; } = false;

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name?.Value}, {nameof(Type)}: {Type?.Value}, {nameof(AutoDelete)}: {AutoDelete}, {nameof(Durable)}: {Durable}.";
        }
    }

    public class QueueOptions
    {
        /// <summary>
        /// Event type. For example: UserNameUpdatedEvent
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// ServiceNameOfConsumer/Exchange.RoutingKey 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Queue auto deleting when there are not consumers 
        /// </summary>
        public bool AutoDelete { get; set; } = true;

        public bool Durable { get; set; } = false;

        public bool Exclusive { get; set; } = true;

        public override string ToString()
        {
            return $"{nameof(RoutingKey)}: {RoutingKey}, {nameof(Name)}: {Name}, {nameof(AutoDelete)}: {AutoDelete}, {nameof(Durable)}: {Durable}, {nameof(Exclusive)}: {Exclusive}.";
        }
    }

    public class ConsumerOptions
    {
        /// <summary>
        /// Limit the number of unacknowledged messages on a channel (or connection) when consuming (aka "prefetch count").
        /// Channel.BasicQos(0, 10, false); Fetch 10 messages per consumer. But all messages handling in order.
        /// Вызов Channel.BasicQos(0, 1, false) означает, что ваш потребитель будет получать только одно готовое сообщение за раз от RabbitMQ, и пока не будет вызван BasicAck , другое сообщение не будет доставлено.
        /// Global flag should be flase, it applies PrefetchCount separately to each new consumer on the channel.
        /// Please note that if your client auto-acks messages, the prefetch value will have no effect.
        /// If you have one single or only a few consumers processing messages quickly, we recommend prefetching many messages at once to keep your client as busy as possible. 
        /// </summary>
        public ushort PrefetchCount { get; set; } = 10;

        /// <summary>
        ///  Channel.BasicAck(ea.DeliveryTag, false);
        ///  Channel.BasicConsume(queue: "queue.1", autoAck: false, consumer: consumer);
        ///  Please note that if your client auto-acks messages, the prefetch value will have no effect.
        /// </summary>
        public bool Autoack { get; set; } = false;

        public override string ToString()
        {
            return $"{nameof(PrefetchCount)}: {PrefetchCount}, {nameof(Autoack)}: {Autoack}.";
        }
    }

    public class RabbitMqEventBusProperties 
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public QueueOptions Queue { get; set; } = new QueueOptions();

        public ConsumerOptions Consumer { get; set; } = new ConsumerOptions();

        public override string ToString()
        {
            return $"{nameof(Exchange)}: {Exchange?.ToString()}, {nameof(Queue)}: {Queue?.ToString()}, {nameof(Consumer)}: {Consumer?.ToString()}.";
        }
    }

    /*
     This is a hard requirement for publishers: sharing a channel (an IModel instance) for concurrent publishing will lead to incorrect
     frame interleaving at the protocol level. Channel instances must not be shared by threads that publish on them.

     If more than one thread needs to access a particular IModel instances, 
     the application should enforce mutual exclusion. 
     One way of achieving this is for all users of an IModel to lock the instance itself.

     If messages A and B were delivered in this order on the same channel, they will be processed in this order.
     */
}
