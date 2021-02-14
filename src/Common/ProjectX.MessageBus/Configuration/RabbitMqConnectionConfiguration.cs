using ProjectX.Core.SeedWork;

namespace ProjectX.MessageBus.Configuration
{
    public class RabbitMqConnectionConfiguration : IOptions
    {
        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public string VirtualHost { get; set; } = "/";

        public string HostName { get; set; } = "localhost";

        public string Port { get; set; } = "5672";
    }
}
