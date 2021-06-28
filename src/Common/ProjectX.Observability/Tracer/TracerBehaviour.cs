using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Observability.Tracer
{
    public class TracerBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ITracer _tracer;

        public TracerBehaviour(ITracer tracer)
        {
            _tracer = tracer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return await _tracer.Trace<TRequest, TResponse>(() => next());
        }
    }
}
