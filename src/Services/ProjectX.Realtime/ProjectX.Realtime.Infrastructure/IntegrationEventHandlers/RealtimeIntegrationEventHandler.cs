using ProjectX.Core.IntegrationEvents;
using ProjectX.RabbitMq;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure.IntegrationEventHandlers
{
    /// <summary>
    /// Handle integration events from RabbitMQ
    /// </summary>
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
