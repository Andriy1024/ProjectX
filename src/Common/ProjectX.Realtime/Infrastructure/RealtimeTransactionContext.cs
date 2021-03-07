using ProjectX.Core;
using ProjectX.RabbitMq;
using System;
using System.Collections.Generic;

namespace ProjectX.Realtime
{
    public sealed class RealtimeTransactionContext : IRealtimeTransactionContext
    {
        private readonly Queue<(RealtimeIntegrationEvent, PublishProperties)> _messages = 
                     new Queue<(RealtimeIntegrationEvent, PublishProperties)>();

        public void Add(RealtimeMessageContext message, IEnumerable<long> receivers) 
        {
            Utill.ThrowIfNull(message, nameof(message));
            Utill.ThrowIfNull(receivers, nameof(receivers));

            var publishProperties = new PublishProperties(
                                        new ExchangeProperties(
                                            name: Exchange.Name.Realtime,
                                            type: Exchange.Type.Fanout,
                                            autoDelete: true,
                                            durable: false));

            var integrationEvent = new RealtimeIntegrationEvent(Guid.NewGuid(), message, receivers);

            _messages.Enqueue((integrationEvent, publishProperties));
        }

        public IEnumerable<(RealtimeIntegrationEvent, PublishProperties)> ExtractMessages() 
        {
            while (_messages.TryDequeue(out var message))
            {
                yield return message;
            }
        }
    }
}
