using ProjectX.Core.SeedWork;

namespace ProjectX.MessageBus.Configuration
{
    public class ResilienceConfiguration : IOptions
    {
        /// <summary>
        /// The number of exceptions that are allowed before opening the circuit breaker.
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 2;

        /// <summary>
        /// The duration the circuit will stay open before resetting. In seconds.
        /// </summary>
        public int DurationOfBreak { get; set; } = 10;

        public int RetryCount { get; set; } = 2;
    }
}
