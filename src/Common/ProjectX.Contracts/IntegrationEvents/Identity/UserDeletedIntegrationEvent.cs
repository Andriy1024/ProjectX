using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.Contracts.IntegrationEvents
{
    public class UserDeletedIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; set; }

        public long UserId { get; set; }

        public UserDeletedIntegrationEvent() {}

        public UserDeletedIntegrationEvent(Guid id, long userId)
        {
            (Id, UserId) = (id, userId);
        }
    }
}
