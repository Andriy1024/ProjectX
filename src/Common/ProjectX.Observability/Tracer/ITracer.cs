using System;
using System.Threading.Tasks;

namespace ProjectX.Observability.Tracer
{
    public interface ITracer
    {
        void Trace(TraceCode code, string description);

        void Trace(Exception error);

        Task<TResult> Trace<TActionType, TResult>(Func<Task<TResult>> func);

        Task<TResult> Trace<TResult>(string actionName, Func<Task<TResult>> func);

        Task Trace<TActionType>(Func<Task> func);

        Task Trace(string actionName, Func<Task> func);
    }
}
