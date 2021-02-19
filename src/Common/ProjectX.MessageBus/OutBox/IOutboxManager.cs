using ProjectX.Core.IntegrationEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    /// <summary>
    /// The manager using db connection and transaction of main app DB context, and used to save Out/In box messages.
    /// </summary>
    public interface IOutboxManager
    {
        /// <summary>
        /// Save the integration event to Outbox.
        /// </summary>
        Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Mark message as sent.
        /// </summary>
        Task MarkAsSent(OutboxMessage message);

        /// <summary>
        /// Check if the integration event already handled.
        /// </summary>
        Task<bool> HasInboxAsync(Guid id);

        /// <summary>
        /// Save the integration event ot Inbox table.
        /// </summary>
        Task AddInboxAsync(IIntegrationEvent integrationEvent);
    }
}
