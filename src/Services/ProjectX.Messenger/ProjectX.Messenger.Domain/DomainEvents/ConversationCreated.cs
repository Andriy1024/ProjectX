using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Messenger.Domain
{
    public sealed class ConversationCreated : IDomainEvent
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Message Message { get; set; }

        public IEnumerable<long> Members { get; set; }

        private ConversationCreated() {}

        public ConversationCreated(Guid id, DateTimeOffset createdAt, IEnumerable<long> members, Message message)
        {
            Id = id;
            CreatedAt = createdAt;
            Members = members;
            Message = message;
        }
    }
}
