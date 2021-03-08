using System.Collections.Generic;

namespace ProjectX.RabbitMq.Pipeline
{
    internal partial class Pipe
    {
        public sealed class Builder<T>
        {
            private readonly Handler<T> _handler;

            private readonly List<Line<T>> _pipes = new List<Line<T>>();

            public Builder(Handler<T> lastPipe)
            {
                _handler = lastPipe;
            }

            public Builder<T> Add(Line<T> pipe)
            {
                _pipes.Add(pipe); return this;
            }

            public Builder<T> Add(IPipeLine<T> pipe)
            {
                _pipes.Add(pipe.Handle); return this;
            }

            private Handler<T> Build(int index)
            {
                if (index < _pipes.Count - 1)
                {
                    return (T request) => _pipes[index](request, Build(index + 1));
                }
                else
                {
                    return (T request) => _pipes[index](request, _handler);
                }
            }

            public Handler<T> Build() 
            {
                if(_pipes.Count == 0) 
                {
                    return _handler;
                }

                return Build(0);
            }
        }
    }
}
