using ProjectX.Core.SeedWork;
using System.Reflection;

namespace ProjectX.MessageBus.Configuration
{
    public class MessageBusConfiguration : IOptions
    {
        /// <summary>
        /// The name of the connecting server. For example: IdentityServer.
        /// </summary>
        public string ConnectionName { get; set; }

        public ResilienceConfiguration Resilience { get; set; }

        public RabbitMqConnectionConfiguration Connection { get; set; }

        public static MessageBusConfiguration Validate(MessageBusConfiguration options)
        {
            options ??= new MessageBusConfiguration()
            {
                ConnectionName = Assembly.GetEntryAssembly().GetName().Name
            };

            options.Connection ??= new RabbitMqConnectionConfiguration();
            options.Resilience ??= new ResilienceConfiguration();

            return options;
        }
    }
}
