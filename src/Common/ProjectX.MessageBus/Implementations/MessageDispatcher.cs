using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core.IntegrationEvents;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Implementations
{
    public class MessageDispatcher : IMessageDispatcher
    {
        readonly IServiceScopeFactory _scopeFactory;

        public MessageDispatcher(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task HandleAsync(IIntegrationEvent integrationEvent)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(integrationEvent);
        }
    }
}
