using Microsoft.Extensions.Logging;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure
{
    public sealed class ChannelQueue<T> : IDisposable
    {
        private readonly ChannelWriter<T> _writer;
        private readonly ChannelReader<T> _reader;
        private readonly ILogger<ChannelQueue<T>> _logger;
        private readonly Func<T, Task> _handler;
        private bool _isDisposed;

        public ChannelQueue(Func<T, Task> handler, ILogger<ChannelQueue<T>> logger)
        {
            var channel = Channel.CreateUnbounded<T>();
            _reader = channel.Reader;
            _writer = channel.Writer;
            _logger = logger;
            _handler = handler;

            Task.Factory.StartNew(ReadChannelAsync, TaskCreationOptions.LongRunning);
        }

        private async Task ReadChannelAsync()
        {
            await foreach (var job in _reader.ReadAllAsync())
            {
                if (_isDisposed) break;

                try
                {
                    await _handler(job);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}, {e.InnerException}, {e.StackTrace}");
                }
            }
        }

        public async ValueTask EnqueueAsync(T job)
        {
            if (_isDisposed)
            {
                _logger.LogError("Trying write to disposed channel.");
                return;
            }

            await _writer.WriteAsync(job);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _writer.Complete();
            _isDisposed = true;
        }
    }
}
