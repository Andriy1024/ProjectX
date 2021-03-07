using ProjectX.RabbitMq;
using System.Collections.Generic;

namespace ProjectX.Realtime
{
    public interface IRealtimeTransactionContext
    {
        void Add(RealtimeMessageContext message, IEnumerable<long> receivers);
        IEnumerable<(RealtimeIntegrationEvent, PublishProperties)> ExtractMessages();
    }
}
