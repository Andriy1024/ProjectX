using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectX.Observability.Tracer
{
    public static class TracerExtensions
    {
        public static IServiceCollection AddTracer(this IServiceCollection services) 
             => services.AddSingleton<ITracer, Tracer>();
        
        public static IServiceCollection AddTracerBehaviour(this IServiceCollection services)
             => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracerBehaviour<,>));
    }
}
