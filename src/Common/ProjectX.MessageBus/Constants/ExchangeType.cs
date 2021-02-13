using ProjectX.Core;

namespace ProjectX.MessageBus
{
    public sealed class ExchangeType : StringEnumeration
    {
        public readonly static ExchangeType Direct = new ExchangeType("direct");
        public readonly static ExchangeType Fanout = new ExchangeType("fanout");

        protected ExchangeType(string value)
            : base(value) { }

        public static implicit operator ExchangeType(string value)
        {
            return FindValue<ExchangeType>(value);
        }
    }
}
