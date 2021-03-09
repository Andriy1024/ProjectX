using MediatR;
using ProjectX.Core.IntegrationEvents;
using ProjectX.RabbitMq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure.IntegrationEventHandlers
{
    //public sealed class RealtimeIntegrationEventHandler : IIntegrationEventHandler<RealtimeIntegrationEvent>
    //{
    //    private readonly WebSocketManager _webSocketManager;

    //    public RealtimeIntegrationEventHandler(WebSocketManager webSocketManager)
    //    {
    //        _webSocketManager = webSocketManager;
    //    }

    //    public async Task<Unit> Handle(RealtimeIntegrationEvent @event, CancellationToken cancellationToken)
    //    {
    //        await _webSocketManager.SendAsync(@event.Message, @event.Receivers);

    //        return Unit.Value;
    //    }
    //}

    public sealed class RealtimeMessageDispatcher : IMessageDispatcher
    {
        private readonly WebSocketManager _webSocketManager;

        public RealtimeMessageDispatcher(WebSocketManager webSocketManager)
        {
            _webSocketManager = webSocketManager;
        }

        public async Task HandleAsync<T>(T integrationEvent) where T : IIntegrationEvent
        {
            if(integrationEvent is RealtimeIntegrationEvent message) 
            {
                await _webSocketManager.SendAsync(message.Message, message.Receivers);
            }
        }
    }
}
