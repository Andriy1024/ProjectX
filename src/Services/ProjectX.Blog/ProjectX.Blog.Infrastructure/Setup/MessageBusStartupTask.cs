using ProjectX.Contracts.IntegrationEvents;
using ProjectX.Core;
using ProjectX.RabbitMq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Setup
{
    public sealed class MessageBusStartupTask : IStartupTask
    {
        private readonly IRabbitMqSubscriber _messageBus;

        public MessageBusStartupTask(IRabbitMqSubscriber messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _messageBus.Subscribe<UserCreatedIntegrationEvent>(o => 
            {
                o.Exchange.Name = Exchange.Name.Identity;
                o.Queue.AutoDelete = false;
                o.Queue.Exclusive = false;
                o.Consumer.Autoack = false;
            });
           
            _messageBus.Subscribe<UserDeletedIntegrationEvent>(o => 
            {
                o.Exchange.Name = Exchange.Name.Identity;
                o.Queue.AutoDelete = false;
                o.Queue.Exclusive = false;
                o.Consumer.Autoack = false;
            });
        }
    }
}
