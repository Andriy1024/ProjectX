using System.Threading.Tasks;

namespace ProjectX.RabbitMq.Pipeline
{
    /// <summary>
    /// Represents interface for <see cref="Pipe.Line{T}">
    /// </summary>
    internal interface IPipeLine<T> 
    {
        Task Handle(T request, Pipe.Handler<T> next);
    }

    /// <summary>
    /// Represents interface for <see cref="Pipe.Handler{T}{T}">
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
