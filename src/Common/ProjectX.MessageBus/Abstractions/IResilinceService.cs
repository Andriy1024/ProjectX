using Polly.CircuitBreaker;

namespace ProjectX.RabbitMq
{
    public sealed class ResilinceService
    {
        private readonly CircuitBreakerPolicy _circuitBreaker;

    }
}
