using System;

namespace ProjectX.Messenger.Domain
{
    public readonly struct ConversationId : IEquatable<ConversationId>
    {
        private const string _keyPrefix = "CONVERSATION";

        public readonly string Value;

        public ConversationId(long user1, long user2)
        { 
            Value = user1 > user2
                          ? $"{_keyPrefix}.{user1}-{user2}"
                          : $"{_keyPrefix}.{user2}-{user1}";
        }

        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object obj) => obj != null && obj is ConversationId key && Equals(key);

        public bool Equals(ConversationId other) => Value.Equals(other.Value);

        public static implicit operator string(ConversationId id) => id.Value;
    }
}
