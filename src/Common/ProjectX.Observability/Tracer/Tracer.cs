using ProjectX.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProjectX.Observability.Tracer
{
    internal class Tracer : ITracer
    {
        public static readonly string Name = "ProjectX";

        private static readonly ActivitySource Source = new ActivitySource(Name);

        public void Trace(TraceCode code, string description)
        {
            var activity = Activity.Current;

            if (activity == null) return;

            activity.AddTag("otel.status_code", code.Code);

            activity.AddTag("otel.status_description", description);
        }

        public void Trace(Exception error)
        {
            Trace(TraceCode.Error, error.Message);
        }

        public Task<TResult> Trace<TActionType,TResult>(Func<Task<TResult>> func) 
        {
            return Trace(typeof(TActionType).Name, func);
        }

        public async Task<TResult> Trace<TResult>(string actionName, Func<Task<TResult>> func)
        {
            using (Activity? activity = Source.StartActivity(actionName))
            {
                try
                {
                    var result = await func();

                    var code = result is IResponse r && !r.IsSuccess
                                      ? TraceCode.Error
                                      : TraceCode.Success;

                    Trace(code, result.ToString());

                    return result;
                }
                catch (Exception ex)
                {
                    Trace(ex);

                    throw;
                }
            }
        }

        public Task Trace<TActionType>(Func<Task> func) 
        {
            return Trace(typeof(TActionType).Name, func);
        }

        public async Task Trace(string actionName, Func<Task> func)
        {
            using (Activity? activity = Source.StartActivity(actionName))
            {
                try
                {
                    await func();

                    Trace(TraceCode.Success, actionName);
                }
                catch (Exception ex)
                {
                    Trace(ex);

                    throw;
                }
            }
        }
    }
}
