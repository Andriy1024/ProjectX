using System;

namespace ProjectX.Messenger.Domain
{
    public struct ConversationId : IEquatable<ConversationId>
    {
        private const string _keyPrefix = "CONVERSATION";

        public string Value { get; }

        public ConversationId(long user1, long user2)
        {
            if (user1 > user2)
            {
                Value = $"{_keyPrefix}.{user1}-{user2}";
            }
            else
            {
                Value = $"{_keyPrefix}.{user2}-{user1}";
            }
        }

        public override string ToString()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ConversationId key && Equals(key);
        }

        public bool Equals(ConversationId other)
        {
            return Value.Equals(other.Value);
        }

        public static implicit operator string(ConversationId id) => id.Value;
    }
}
