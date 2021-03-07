using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Messenger.Domain
{
    public sealed class ConversationStarted : IDomainEvent
    {
        public string Id { get; set; }

        public IEnumerable<long> Users { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        private ConversationStarted() {}

        public ConversationStarted(string id, DateTimeOffset createdAt, IEnumerable<long> users)
        {
            Id = id;
            CreatedAt = createdAt;
            Users = users;
        }
    }
}
