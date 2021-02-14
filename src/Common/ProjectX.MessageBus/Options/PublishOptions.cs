namespace ProjectX.MessageBus
{
    public class PublishOptions
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public string RoutingKey { get; set; }

        /// <summary>
        /// Need to investigate
        /// basicProperties.Persistent 
        /// </summary>
        //public bool Persistent { get; set; }
    }
}
