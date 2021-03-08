using ProjectX.Core.IntegrationEvents;
using System;
using System.Collections.Generic;

namespace ProjectX.Realtime
{
    public class RealtimeIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; set; }

        public RealtimeMessageContext Message { get; set; }

        public IEnumerable<long> Receivers { get; set; }

        public RealtimeIntegrationEvent()
        {
        }

        public RealtimeIntegrationEvent(Guid id, RealtimeMessageContext message, IEnumerable<long> receivers)
        {
            Id = id;
            Message = message;
            Receivers = receivers;
        }
    }
}
