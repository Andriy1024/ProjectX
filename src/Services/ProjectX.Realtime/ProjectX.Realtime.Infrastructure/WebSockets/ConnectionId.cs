using System;

namespace ProjectX.Realtime.Infrastructure
{
    /// <summary>
    /// Represents the identifier of the <see cref="WebSocketContext"/>.
    /// </summary>
    public readonly struct ConnectionId : IEquatable<ConnectionId>
    {
        public ConnectionId(Guid value)
        {
            if (Guid.Empty == value)
            {
                throw new ArgumentException($"Invalid connection id: {value}");
            }

            Value = value.ToString();
        }

        public ConnectionId(string value)
        {
            if (!Guid.TryParse(value, out Guid id) || Guid.Empty == id)
            {
                throw new ArgumentException($"Invalid connection id: {value}");
            }

            Value = id.ToString();
        }

        public string Value { get; }

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
            if (ReferenceEquals(obj, null)) return false;

            return obj switch
            {
                ConnectionId id => Equals(id),
                Guid id => Equals(id),
                string id => Equals(id),
                _ => false
            };
        }

        public bool Equals(ConnectionId other)
        {
            return Equals(other.Value);
        }

        public bool Equals(Guid other)
        {
            return Equals(other.ToString());
        }

        public bool Equals(string other)
        {
            return Value.Equals(other);
        }

        public static implicit operator string(ConnectionId id) => id.ToString();
        public static implicit operator Guid(ConnectionId id) => Guid.Parse(id.Value);
        public static implicit operator ConnectionId(string id) => new ConnectionId(id);
        public static implicit operator ConnectionId(Guid id) => new ConnectionId(id);

        public static bool operator ==(ConnectionId x, ConnectionId y) => Equals(x, y);
        public static bool operator !=(ConnectionId x, ConnectionId y) => !Equals(x, y);

        public static bool operator ==(string x, ConnectionId y) => Equals(x, y);
        public static bool operator !=(string x, ConnectionId y) => !Equals(x, y);
        public static bool operator ==(ConnectionId x, string y) => Equals(x, y);
        public static bool operator !=(ConnectionId x, string y) => !Equals(x, y);

        public static bool operator ==(Guid x, ConnectionId y) => Equals(x, y);
        public static bool operator !=(Guid x, ConnectionId y) => !Equals(x, y);
        public static bool operator ==(ConnectionId x, Guid y) => Equals(x, y);
        public static bool operator !=(ConnectionId x, Guid y) => !Equals(x, y);
    }
}
