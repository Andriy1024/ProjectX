using ProjectX.Core;

namespace ProjectX.Messenger.Domain
{
    public sealed class ParticipantHistoryCleared : IDomainEvent
    {
        public string ConversationId { get; set; }
        public long Participant { get; set; }

        public ParticipantHistoryCleared() {}

        public ParticipantHistoryCleared(ConversationId conversationId, long participant)
        {
            ConversationId = conversationId;
            Participant = participant;
        }
    }
}
