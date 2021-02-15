using ProjectX.Contracts.IntegrationEvents;
using ProjectX.Core;
using ProjectX.MessageBus;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Setup
{
    public sealed class MessageBusStartupTask : IStartupTask
    {
        private readonly IMessageBus _messageBus;

        public MessageBusStartupTask(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _messageBus.Subscribe<UserCreatedIntegrationEvent>(new SubscribeProperties() 
            { 
                Exchange = new ExchangeOptions(MessageBusExchanges.Identity)
            });
        }
    }
}
