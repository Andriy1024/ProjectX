namespace ProjectX.RabbitMq
{
    public class QueueProperties
    {
        /// <summary>
        /// Event type. For example: UserNameUpdatedEvent
        /// </summary>
        public string RoutingKey { get; set; } = "";

        /// <summary>
        /// ServiceNameOfConsumer/Exchange.RoutingKey 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Queue auto deleting when there are not consumers. 
        /// </summary>
        public bool AutoDelete { get; set; } = true;

        public bool Durable { get; set; } = false;

        /// <summary>
        /// Used by only one connection and the queue will be deleted when that connection closes.
        /// </summary>
        public bool Exclusive { get; set; } = true;

        public override string ToString()
        {
            return $"{nameof(RoutingKey)}: {RoutingKey}, {nameof(Name)}: {Name}, {nameof(AutoDelete)}: {AutoDelete}, {nameof(Durable)}: {Durable}, {nameof(Exclusive)}: {Exclusive}.";
        }
    }
}
