using ProjectX.Core.IntegrationEvents;
using System.Collections.Generic;

namespace ProjectX.Core.Realtime
{
    /// <summary>
    /// Integration event for sending realtime messages to event bus.
    /// </summary>
    public interface IRealtimeIntegrationEvent : IIntegrationEvent
    {
        /// <summary>
        /// Realtime message for clients.
        /// </summary>
        IRealtimeMessage Message { get; }

        /// <summary>
        /// The collection of ids of target receivers (human resource ids) for this message.
        /// </summary>
        IEnumerable<long> Receivers { get; }
    }
}
