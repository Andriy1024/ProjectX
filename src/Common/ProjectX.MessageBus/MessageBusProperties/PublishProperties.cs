using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.MessageBus
{
    public class PublishProperties : IEventBusProperties
    {
        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();

        public string RoutingKey { get; set; }

        public static PublishProperties Validate(IEventBusProperties properties, bool allowEmptyRoutingKey = false) 
        {
            Utill.ThrowIfNull(properties, nameof(properties));

            if (!(properties is PublishProperties publishOptions))
                  throw new ArgumentException($"{nameof(properties)} should be {nameof(PublishProperties)} type.");

            ExchangeOptions.Validate(publishOptions.Exchange);

            if(!allowEmptyRoutingKey)
                Utill.ThrowIfNullOrEmpty(publishOptions.RoutingKey, nameof(publishOptions.RoutingKey));

            return publishOptions;
        }

        /// <summary>
        /// Need to investigate
        /// basicProperties.Persistent 
        /// </summary>
        //public bool Persistent { get; set; }
    }
}
