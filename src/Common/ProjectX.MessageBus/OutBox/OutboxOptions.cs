using ProjectX.Core.SeedWork;

namespace ProjectX.MessageBus.Outbox
{
    public class OutboxOptions : IOptions
    {
        public string Exchange { get; set; }
        public int IntervalMilliseconds { get; set }
    }
}
