using ProjectX.Core.IntegrationEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    /// <summary>
    /// This pattern ensures that each message is published at least once.
    /// </summary>
    public interface IOutboxManager
    {
        /// <summary>
        /// Persist the message to data stote in local transaction, to 
        /// </summary>
        Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Using in background worker to publish pending messages to message bus.
        /// </summary>
        Task<OutboxMessage[]> RetrievePendingMessageAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Mark message as sent.
        /// </summary>
        Task MarkAsSent(OutboxMessage message);

        /// <summary>
        /// Check if message already handled.
        /// </summary>
        Task<bool> HasInboxAsync(Guid id);

        /// <summary>
        /// Save general information about handled message.
        /// </summary>
        Task AddInboxAsync(IIntegrationEvent integrationEvent);
    }
}
