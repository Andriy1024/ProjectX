namespace ProjectX.MessageBus
{
    public class PublishOptions
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public string RoutingKey { get; set; }
    }
}
