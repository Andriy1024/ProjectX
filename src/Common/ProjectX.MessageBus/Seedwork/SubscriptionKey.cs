using System;

namespace ProjectX.RabbitMq
{
    /// <summary>
    /// The structure is used as the key for subscribers' and publishers' collections.
    /// </summary>
    public readonly struct SubscriptionKey
    {
        private readonly string _exchange;
        
        private readonly string _routingKey;

        public SubscriptionKey(string exchange, string routingKey)
        {
            _exchange = exchange;
            
            _routingKey = routingKey;
        }

        public override string ToString()
        {
            return $"{_exchange}.{_routingKey}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_exchange, _routingKey);
        }
    }
}
