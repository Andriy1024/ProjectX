using ProjectX.Core.IntegrationEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    /// <summary>
    /// The service using db connection and transaction of main app DB context, and used to save Out/Inbox messages in the scope of the request's transaction.
    /// </summary>
    public interface IOutboxTransactionContext
    {
        /// <summary>
        /// Save the integration event to Outbox.
        /// </summary>
        Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if the integration event already handled.
        /// </summary>
        Task<bool> HasInboxAsync(Guid id);

        /// <summary>
        /// Save the integration event to Inbox table.
        /// </summary>
        Task AddInboxAsync(IIntegrationEvent integrationEvent);

        /// <summary>
        /// The action is triggered in TransactionCommitedOutboxHandler, to immediate notify publisher about new messages.
        /// </summary>
        Task OnTransactionCommitedAsync();
    }
}
