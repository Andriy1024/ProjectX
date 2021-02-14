using System;

namespace ProjectX.Core.Realtime
{
    public sealed class RealtimeMessage : IRealtimeMessage
    {
        public string Type { get; set; }

        public object Payload { get; set; }

        public RealtimeMessage() { }

        public RealtimeMessage(object messagePayload)
        {
            Payload = messagePayload ?? throw new ArgumentNullException(nameof(messagePayload));
            Type = messagePayload.GetType().Name;
        }
    }
}
