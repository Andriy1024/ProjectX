using RabbitMQ.Client;

namespace ProjectX.MessageBus
{
    public class Publisher 
    {
        public Publisher(ExchangeOptions exchange, string routingKey, IModel channel)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Channel = channel;
        }

        public ExchangeOptions Exchange { get; }
        public string RoutingKey { get; }
        public IModel Channel { get; }

        private object _sync = new object();

        public void Publish(IBasicProperties properties, byte[] message) 
        {
            //properties ??= Channel.CreateBasicProperties(); //neet to investigate
            lock (_sync) 
            {
                Channel.BasicPublish(Exchange.Name.Value, RoutingKey, properties, message);
            }
        }

        public void Close() 
        {
            lock (_sync) 
            {
                if (!Channel.IsClosed) 
                {
                    Channel.Close();
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(Exchange)}: {Exchange.ToString()}, {nameof(RoutingKey)}: {RoutingKey}.";
        }
    }
}
