using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;

namespace ProjectX.RabbitMq
{
    public class PublishProperties 
    {
        public ExchangeProperties Exchange { get; }

        public string RoutingKey { get; set; } = "";

        public bool EnableRetryPolicy { get; set; }

        public PublishProperties()
        {
            Exchange = new ExchangeProperties();
        }

        public PublishProperties(Exchange.Name exchangeName)
        {
            Exchange.Name = exchangeName;
        }

        public PublishProperties(ExchangeProperties exchange)
        {
            Utill.ThrowIfNull(exchange, nameof(exchange));

            Exchange = exchange;
        }

        public static PublishProperties Validate(PublishProperties properties, bool allowEmptyRoutingKey = false) 
        {
            Utill.ThrowIfNull(properties, nameof(properties));

            ExchangeProperties.Validate(properties.Exchange);

            if (!allowEmptyRoutingKey) 
            {
                Utill.ThrowIfNullOrEmpty(properties.RoutingKey, nameof(properties.RoutingKey));
            }
                
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
