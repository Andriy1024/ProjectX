using System.Threading.Tasks;

namespace ProjectX.RabbitMq.Pipeline
{
    /*
      The pattern represets a chain of multiple handlers to build an outgoing request middleware pipeline.
      Each of these handlers is able to perform work before and after the outgoing request. This pattern:
      Is similar to the inbound middleware pipeline in ASP.NET Core.
      Provides a mechanism to manage cross-cutting concerns around RabbitMq subscriber requests, such as:
        - error handling
        - retrying
        - logging
        - etc...
     */

    /// <summary>
    /// Interface for <see cref="Pipe.Line{T}">
    /// </summary>
    internal interface IPipeLine<T> 
    {
        Task Handle(T request, Pipe.Handler<T> next);
    }

    /// <summary>
    /// Interface for <see cref="Pipe.Handler{T}{T}">
    /// </summary>
    internal interface IPipeHandler<T> 
    {
        Task Handle(T request);
    }

    internal partial class Pipe 
    {
        public delegate Task Line<T>(T request, Handler<T> next);

        public delegate Task Handler<T>(T request);
    }
}
