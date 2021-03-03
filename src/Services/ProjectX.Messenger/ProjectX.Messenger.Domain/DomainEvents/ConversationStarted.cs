using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Domain
{
    public sealed class ConversationStarted : IDomainEvent
    {
        public string Id { get; set; }

        public long FirstParticipant { get; set; }

        public long SecondParticipant { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        private ConversationStarted() {}

        public ConversationStarted(ConversationId id, DateTimeOffset createdAt, long firstParticipant, long secondParticipant)
        {
            Id = id;
            CreatedAt = createdAt;
            FirstParticipant = firstParticipant;
            SecondParticipant = secondParticipant;
        }
    }
}
