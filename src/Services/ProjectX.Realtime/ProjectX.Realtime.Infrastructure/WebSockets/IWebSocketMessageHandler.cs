using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure
{
    public interface IWebSocketMessageHandler
    {
        Task HandleMessageAsync(WebSocketMessage message);
        Task HandleDisconnectionAsync(WebSocketConnection connection);
    }
}
