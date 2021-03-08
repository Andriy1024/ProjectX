using ProjectX.Core;

namespace ProjectX.RabbitMq
{
    public sealed class SubscribeProperties
    {
        public ExchangeProperties Exchange { get; set; } = new ExchangeProperties();

        public QueueProperties Queue { get; set; } = new QueueProperties();

        public ConsumerProperties Consumer { get; set; } = new ConsumerProperties();

        public override string ToString()
        {
            return $"{nameof(Exchange)}: {Exchange?.ToString()}, {nameof(Queue)}: {Queue?.ToString()}, {nameof(Consumer)}: {Consumer?.ToString()}.";
        }

        public static SubscribeProperties Validate(SubscribeProperties properties)
        {
            Utill.ThrowIfNull(properties, nameof(properties));

            ExchangeProperties.Validate(properties.Exchange);

            return properties;
        }
    }
}
