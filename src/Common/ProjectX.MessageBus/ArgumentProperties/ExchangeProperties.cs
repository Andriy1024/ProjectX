using ProjectX.Core;

namespace ProjectX.RabbitMq
{
    public class ExchangeProperties 
    {
        public Exchange.Name Name { get; set; }

        public Exchange.Type Type { get; set; } = Exchange.Type.Direct;

        public bool AutoDelete { get; set; } = true;

        public bool Durable { get; set; } = false;

        public bool IsFanout => Type?.Value == Exchange.Type.Fanout;

        public ExchangeProperties() {}

        public ExchangeProperties(Exchange.Name name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name?.Value}, {nameof(Type)}: {Type?.Value}, {nameof(AutoDelete)}: {AutoDelete}, {nameof(Durable)}: {Durable}.";
        }

        public static ExchangeProperties Validate(ExchangeProperties exchange) 
        {
            Utill.ThrowIfNull(exchange, nameof(exchange));
            Utill.ThrowIfNull(exchange.Name, nameof(exchange.Name));
            Utill.ThrowIfNull(exchange.Type, nameof(exchange.Type));
            return exchange;
        }
    }
}
