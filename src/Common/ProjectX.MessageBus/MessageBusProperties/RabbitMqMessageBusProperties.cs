namespace ProjectX.MessageBus
{
    public class RabbitMqMessageBusProperties 
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public QueueOptions Queue { get; set; } = new QueueOptions();

        public ConsumerOptions Consumer { get; set; } = new ConsumerOptions();

        public override string ToString()
        {
            return $"{nameof(Exchange)}: {Exchange?.ToString()}, {nameof(Queue)}: {Queue?.ToString()}, {nameof(Consumer)}: {Consumer?.ToString()}.";
        }
    }
}
