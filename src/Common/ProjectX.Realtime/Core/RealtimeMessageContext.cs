using System;

namespace ProjectX.Realtime
{
    public class RealtimeMessageContext
    {
        public string Type { get; set; }

        public object Message { get; set; }

        public RealtimeMessageContext()
        {
        }

        public RealtimeMessageContext(IRealtimeMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Message = message;
            Type = Message.GetType().Name;
        }
    }
}
