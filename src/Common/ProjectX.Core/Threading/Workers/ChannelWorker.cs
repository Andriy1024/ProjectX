using Microsoft.Extensions.Logging;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ProjectX.Core.Threading.Workers
{
    public class ChannelWorker<T> : IDisposable
    {
        readonly ChannelWriter<T> _writer;
        readonly ChannelReader<T> _reader;
        readonly ILogger<ChannelWorker<T>> _logger;
        readonly Func<T, Task> _handler;
        bool _isDisposed;

        public ChannelWorker(Func<T, Task> handler, ILogger<ChannelWorker<T>> logger)
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
            while (await _reader.WaitToReadAsync())
            {
                var job = await _reader.ReadAsync();

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
                _logger.LogWarning("Trying write to disposed channel.");
                
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
