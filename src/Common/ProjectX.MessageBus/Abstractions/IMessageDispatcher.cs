using ProjectX.Core.IntegrationEvents;
using System.Threading.Tasks;

namespace ProjectX.MessageBus
{
    /// <summary>
    /// Responsible for dispatch events received from messages broker.
    /// </summary>
    public interface IMessageDispatcher
    {
        Task HandleAsync(IIntegrationEvent integrationEvent);
    }
}
