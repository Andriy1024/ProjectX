using ProjectX.Core;
using ProjectX.Core.SeedWork;
using ProjectX.MessageBus;

namespace ProjectX.Outbox
{
    public class OutboxOptions : IOptions
    {
        public string Exchange { get; set; }

        public int IntervalMilliseconds { get; set; } = 2000;

        public static OutboxOptions Validate(OutboxOptions outboxOptions) 
        {
            Utill.ThrowIfNull(outboxOptions, nameof(outboxOptions));
            
            Exchange.Name exchanges = outboxOptions.Exchange;

            return outboxOptions;
        }
    }
}
