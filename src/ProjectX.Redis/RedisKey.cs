using System;
using System.Linq;

namespace ProjectX.Redis
{
    public readonly struct RedisKey : IEquatable<RedisKey>
    {
        public string Value { get; }

        private const string _separator = "_";

        public RedisKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            Value = key;
        }

        public RedisKey(params string[] keys)
        {
            if (keys == null || !keys.Any())
                throw new ArgumentNullException("keys");

            Value = string.Join(_separator, keys);
        }

        public RedisKey Append(RedisKey suffix) => new RedisKey(Value, suffix.Value);
        public RedisKey Append(string suffix) => new RedisKey(Value, suffix);

        public bool Equals(RedisKey other) => Value.Equals(other.Value);

        public override bool Equals(object obj)
        {
            if (obj is RedisKey key)
                return Equals(key);

            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static implicit operator string(RedisKey key) => key.Value;
        public static implicit operator RedisKey(string key) => new RedisKey(key);
        public static bool operator ==(RedisKey x, RedisKey y) => Equals(x, y);
        public static bool operator !=(RedisKey x, RedisKey y) => !Equals(x, y);
        public static bool operator ==(string x, RedisKey y) => Equals(x, y);
        public static bool operator !=(string x, RedisKey y) => !Equals(x, y);
        public static bool operator ==(RedisKey x, string y) => Equals(x, y);
        public static bool operator !=(RedisKey x, string y) => !Equals(x, y);
    }
}
