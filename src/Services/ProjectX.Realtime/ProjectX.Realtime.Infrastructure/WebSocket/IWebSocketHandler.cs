using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure
{
    public interface IWebSocketHandler
    {
        Task HandleMessageAsync(WebSocketMessage message);
        Task HandleDisconnectionAsync(WebSocketConnection connection);
    }
}
