using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;

namespace ProjectX.RabbitMq
{
    public class PublishProperties 
    {
        public ExchangeProperties Exchange { get; set; } = new ExchangeProperties();

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

            ExchangeProperties.Validate(properties.Exchange);

            if(!allowEmptyRoutingKey)
                Utill.ThrowIfNullOrEmpty(properties.RoutingKey, nameof(properties.RoutingKey));

            return properties;
        }

        public static string CreateRoutingKey<T>(T integrationEvent)
            where T : IIntegrationEvent
        { 
            return integrationEvent.GetType().Name;
        }

        /// <summary>
        /// Need to investigate
        /// basicProperties.Persistent 
        /// </summary>
        //public bool Persistent { get; set; }
    }
}
