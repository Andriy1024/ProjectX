using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageDeleted : IDomainEvent
    {
        public string ConversationId { get; set; }

        public Guid MessageId { get; set; }

        public MessageDeleted() {}

        public MessageDeleted(ConversationId conversationId, Guid messageId)
        {
            ConversationId = conversationId;
            MessageId = messageId;
        }
    }
}
