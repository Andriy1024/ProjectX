using System.Collections.Generic;

namespace ProjectX.RabbitMq.Pipeline
{
    internal partial class Pipe
    {
        public sealed class Builder<T>
        {
            private readonly Handler<T> _handler;

            private readonly List<Pipe.Line<T>> _pipes = new List<Line<T>>();

            public Builder(Handler<T> lastPipe)
            {
                _handler = lastPipe;
            }

            public Builder<T> Add(Pipe.Line<T> pipe)
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
                    return (message) => _pipes[index](message, Build(index + 1));
                }
                else
                {
                    return (message) => _pipes[index](message, _handler);
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
