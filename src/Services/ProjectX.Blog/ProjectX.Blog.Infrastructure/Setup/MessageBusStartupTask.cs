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
            _messageBus.Subscribe<UserCreatedIntegrationEvent>(new SubscribeProperties() 
            { 
                Exchange = new ExchangeProperties(Exchange.Name.Identity)
            });
        }
    }
}
