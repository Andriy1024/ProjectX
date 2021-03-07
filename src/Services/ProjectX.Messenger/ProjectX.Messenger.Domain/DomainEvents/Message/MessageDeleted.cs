using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Messenger.Domain
{
    public sealed class MessageDeleted : IDomainEvent
    {
        public string ConversationId { get; set; }

        public IEnumerable<long> Users { get; set; }

        public Guid MessageId { get; set; }

        public MessageDeleted() {}

        public MessageDeleted(ConversationId conversationId, IEnumerable<long> users, Guid messageId)
        {
            ConversationId = conversationId;
            Users = users;
            MessageId = messageId;
        }
    }
}
