using ProjectX.Core;
using ProjectX.RabbitMq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Infrastructure.Setup
{
    public sealed class MessageBusStartupTask : IStartupTask
    {
        private readonly IRabbitMqConnectionService _rabbitMq;

        public MessageBusStartupTask(IRabbitMqConnectionService rabbitMq)
        {
            _rabbitMq = rabbitMq;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (!_rabbitMq.TryConnect()) 
            {
                throw new Exception("Rabbitmq unreachable.");
            }

            return Task.CompletedTask;
        }
    }
}
