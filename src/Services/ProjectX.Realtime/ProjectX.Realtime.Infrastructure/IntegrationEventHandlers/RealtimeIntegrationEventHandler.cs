using MediatR;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure.IntegrationEventHandlers
{
    public sealed class RealtimeIntegrationEventHandler : IIntegrationEventHandler<RealtimeIntegrationEvent>
    {
        private readonly WebSocketConnectionManager _webSocketManager;

        public RealtimeIntegrationEventHandler(WebSocketConnectionManager webSocketManager)
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
