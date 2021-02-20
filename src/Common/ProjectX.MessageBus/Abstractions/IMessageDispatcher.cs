using ProjectX.Core.IntegrationEvents;
using System.Threading.Tasks;

namespace ProjectX.RabbitMq
{
    /// <summary>
    /// Implementation of the interface is responsible for dispatching events received from messages broker.
    /// </summary>
    public interface IMessageDispatcher
    {
        Task HandleAsync<T>(T integrationEvent) where T : IIntegrationEvent;
    }
}
