using MediatR;

namespace ProjectX.Core.IntegrationEvents
{
    public interface IIntegrationEventHandler<TEvent> : IRequestHandler<TEvent>
        where TEvent : IIntegrationEvent
    {
    }
}
