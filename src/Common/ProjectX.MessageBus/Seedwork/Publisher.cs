using RabbitMQ.Client;

namespace ProjectX.RabbitMq
{
    public sealed class Publisher 
    {
        public Publisher(ExchangeProperties exchange, string routingKey, IModel channel)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Channel = channel;
        }

        public ExchangeProperties Exchange { get; }
        public string RoutingKey { get; }
        public IModel Channel { get; }

        private object _sync = new object();

        /// <summary>
        /// This is a hard requirement for publishers: sharing a channel(an IModel instance) for concurrent publishing will lead to incorrect frame interleaving at the protocol level.
        /// Channel instances must not be shared by threads that publish on them.
        /// If more than one thread needs to access a particular IModel instances, the application should enforce mutual exclusion. 
        /// One way of achieving this is for all users of an IModel to lock the instance.
        /// </summary>
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
