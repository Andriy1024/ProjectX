using ProjectX.Core.SeedWork;

namespace ProjectX.MessageBus
{
    public class RabbitMqConnectionOptions : IOptions
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public string HostName { get; set; }

        public string Port { get; set; }
    }

    public class MessageBusOptions : IOptions 
    {
        /// <summary>
        /// The name of the connecting server. For example: IdentityServer.
        /// </summary>
        public string ConnectionName { get; set; }

        public ResilienceOptions Resilience { get; set; }

        public RabbitMqConnectionOptions Connection { get; set; }

        public static MessageBusOptions Validate(MessageBusOptions options) 
        {
            return options;
        }
    }

    public class ResilienceOptions 
    {
        /// <summary>
        /// The number of exceptions that are allowed before opening the circuit breaker.
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 2;

        /// <summary>
        /// The duration the circuit will stay open before resetting. In seconds.
        /// </summary>
        public int DurationOfBreak { get; set; } = 10;

        public int RetryCount { get; set; } = 2;
    }
}
