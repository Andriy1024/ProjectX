﻿using ProjectX.Core.SeedWork;

namespace ProjectX.MessageBus
{
    public sealed class RabbitMqConnectionOptions : IOptions
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public string HostName { get; set; }

        public string Port { get; set; }
    }
}