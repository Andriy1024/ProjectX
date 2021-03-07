using ProjectX.Core.IntegrationEvents;
using System;
using System.Collections.Generic;

namespace ProjectX.Realtime
{
    public class RealtimeIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; set; }

        RealtimeMessageContext Message { get; set; }

        IEnumerable<long> Receivers { get; set; }

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

    public class RealtimeMessageContext
    {
        string Type { get; set; }

        object Message { get; set; }

        public RealtimeMessageContext()
        {
        }

        public RealtimeMessageContext(IRealtimeMessage message)
        {
            if(message == null) throw new ArgumentNullException(nameof(message));
            
            Message = message;
            Type = Message.GetType().Name;
        }
    }

    public interface IRealtimeMessage 
    {
    }
}
