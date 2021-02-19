using ProjectX.Core;

namespace ProjectX.MessageBus
{
    public class SubscribeProperties
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public QueueOptions Queue { get; set; } = new QueueOptions();

        public ConsumerOptions Consumer { get; set; } = new ConsumerOptions();

        public override string ToString()
        {
            return $"{nameof(Exchange)}: {Exchange?.ToString()}, {nameof(Queue)}: {Queue?.ToString()}, {nameof(Consumer)}: {Consumer?.ToString()}.";
        }

        public static SubscribeProperties Validate(SubscribeProperties properties)
        {
            Utill.ThrowIfNull(properties, nameof(properties));

            ExchangeOptions.Validate(properties.Exchange);

            return properties;
        }
    }
}
