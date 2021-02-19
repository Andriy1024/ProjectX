using ProjectX.Core;

namespace ProjectX.MessageBus
{
    public class PublishProperties 
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public string RoutingKey { get; set; }

        public PublishProperties()
        {
        }

        public PublishProperties(Exchange.Name exchangeName)
        {
            Exchange.Name = exchangeName;
        }

        public static PublishProperties Validate(PublishProperties properties, bool allowEmptyRoutingKey = false) 
        {
            Utill.ThrowIfNull(properties, nameof(properties));

            ExchangeOptions.Validate(properties.Exchange);

            if(!allowEmptyRoutingKey)
                Utill.ThrowIfNullOrEmpty(properties.RoutingKey, nameof(properties.RoutingKey));

            return properties;
        }

        /// <summary>
        /// Need to investigate
        /// basicProperties.Persistent 
        /// </summary>
        //public bool Persistent { get; set; }
    }
}
