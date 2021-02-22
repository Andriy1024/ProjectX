using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.Outbox
{
    /// <summary>
    /// The message persists to a data store in a local transaction, to ensure the message will be published to a message bus at least once.
    /// </summary>
    public sealed class OutboxMessage
    {
        /// <summary>
        /// Integration event id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The property represents CLR type name of the integration event.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The property contains the integration event in JSON format.
        /// </summary>
        public string SerializedMessage { get; set; }

        /// <summary>
        /// Not saved in DB. The property contains the integration event.
        /// </summary>
        public IIntegrationEvent Message { get; set; }

        /// <summary>
        /// Not saved in DB. The property contains CLR type of the integration event.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The property represents the date-time of saving to DB.
        /// </summary>
        public DateTime SavedAt { get; set; }

        /// <summary>
        /// The property is initialized when the message is sent.
        /// </summary>
        public DateTime? SentAt { get; set; }

        private OutboxMessage()
        {
        }

        public OutboxMessage(IIntegrationEvent message, string serializedMessage, DateTime savedAt)
        {
            Id = message.Id;
            Message = message;
            Type = message.GetType();
            MessageType = Type.AssemblyQualifiedName;
            SerializedMessage = serializedMessage;
            SavedAt = savedAt;
        }
    }
}
