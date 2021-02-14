using ProjectX.Core;

namespace ProjectX.MessageBus
{
    public sealed class MessageBusExchanges : StringEnumeration
    {
        public readonly static MessageBusExchanges Identity = new MessageBusExchanges("ProjectX.Identity");
        public readonly static MessageBusExchanges Realtime = new MessageBusExchanges("ProjectX.Realtime");
        public readonly static MessageBusExchanges Blog = new MessageBusExchanges("ProjectX.Blog");

        protected MessageBusExchanges(string value)
            : base(value) { }

        public static implicit operator MessageBusExchanges(string value)
        {
            return FindValue<MessageBusExchanges>(value);
        }
    }
}
