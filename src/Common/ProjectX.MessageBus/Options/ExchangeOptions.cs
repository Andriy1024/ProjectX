namespace ProjectX.MessageBus
{
    public class ExchangeOptions
    {
        public EventBusExchanges Name { get; set; }

        public ExchangeType Type { get; set; } = ExchangeType.Direct;

        public bool AutoDelete { get; set; } = true;

        public bool Durable { get; set; } = false;

        public bool IsFanout => Type?.Value == ExchangeType.Fanout;

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name?.Value}, {nameof(Type)}: {Type?.Value}, {nameof(AutoDelete)}: {AutoDelete}, {nameof(Durable)}: {Durable}.";
        }
    }
}
