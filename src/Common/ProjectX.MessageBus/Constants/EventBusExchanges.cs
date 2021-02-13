using ProjectX.Core;

namespace ProjectX.MessageBus
{
    public sealed class EventBusExchanges : StringEnumeration
    {
        public readonly static EventBusExchanges Identity = new EventBusExchanges("IDENTITY");
        public readonly static EventBusExchanges Realtime = new EventBusExchanges("REALTIME");
        public readonly static EventBusExchanges Blog = new EventBusExchanges("BLOG");


        protected EventBusExchanges(string value)
            : base(value) { }

        public static implicit operator EventBusExchanges(string value)
        {
            return FindValue<EventBusExchanges>(value);
        }
    }
}
