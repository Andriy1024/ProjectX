namespace ProjectX.MessageBus
{
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
