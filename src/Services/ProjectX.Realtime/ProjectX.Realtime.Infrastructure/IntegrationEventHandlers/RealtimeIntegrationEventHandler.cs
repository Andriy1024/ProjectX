using MediatR;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure.IntegrationEventHandlers
{
    public sealed class RealtimeIntegrationEventHandler : IIntegrationEventHandler<RealtimeIntegrationEvent>
    {
        private readonly WebSocketManager _webSocketManager;

        public RealtimeIntegrationEventHandler(WebSocketManager webSocketManager)
        {
            _webSocketManager = webSocketManager;
        }

        public async Task<Unit> Handle(RealtimeIntegrationEvent @event, CancellationToken cancellationToken)
        {
            await _webSocketManager.SendAsync(@event.Message, @event.Receivers);

            return Unit.Value;
        }
    }
}
