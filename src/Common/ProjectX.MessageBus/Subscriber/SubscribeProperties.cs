using ProjectX.Core;

namespace ProjectX.RabbitMq
{
    public sealed class SubscribeProperties
    {
        public ExchangeProperties Exchange { get; } = new ExchangeProperties();

        public QueueProperties Queue { get; } = new QueueProperties();

        public ConsumerProperties Consumer { get; } = new ConsumerProperties();

        public override string ToString()
        {
            return $"{nameof(Exchange)}: {Exchange.ToString()}, {nameof(Queue)}: {Queue.ToString()}, {nameof(Consumer)}: {Consumer.ToString()}.";
        }

        public static SubscribeProperties Validate(SubscribeProperties properties)
        {
            Utill.ThrowIfNull(properties, nameof(properties));

            ExchangeProperties.Validate(properties.Exchange);

            return properties;
        }
    }
}
