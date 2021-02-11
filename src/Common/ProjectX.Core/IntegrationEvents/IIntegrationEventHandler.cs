using MediatR;

namespace ProjectX.Core.IntegrationEvents
{
    public interface IIntegrationEventHandler<TEvent> : INotificationHandler<TEvent>
        where TEvent : IIntegrationEvent
    {
    }
}
