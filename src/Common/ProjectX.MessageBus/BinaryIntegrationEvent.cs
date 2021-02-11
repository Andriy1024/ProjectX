using ProjectX.Core.IntegrationEvents;

namespace ProjectX.MessageBus
{
    public sealed class BinaryIntegrationEvent : IIntegrationEvent
    {
        public BinaryIntegrationEvent() { }

        public BinaryIntegrationEvent(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; set; }
    }
}
