using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    public class OutboxMessagePublisher : IHostedService
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxMessagePublisher> _logger;
        private readonly OutboxOptions _options;
        private readonly MessageBusExchanges _exchange;
        private readonly TimeSpan _interval;

        public OutboxMessagePublisher(IMessageBus messageBus, 
            IServiceScopeFactory scopeFactory, 
            IOptions<OutboxOptions> options,
            ILogger<OutboxMessagePublisher> logger)
        {
            _messageBus = messageBus;
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _exchange = _options.Exchange;
            _interval = TimeSpan.FromMilliseconds(_options.IntervalMilliseconds);
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay((int)TimeSpan.FromSeconds(10).TotalMilliseconds, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var jobId = Guid.NewGuid().ToString("N");
                
                _logger.LogTrace($"Started sending outbox messages... [job id: '{jobId}']");
                
                var stopwatch = new Stopwatch();
                
                stopwatch.Start();

                try
                {
                    await SendOutboxMessagesAsync(cancellationToken);
                }
                catch (OperationCanceledException) 
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }

                stopwatch.Stop();

                _logger.LogTrace($"Finished sending outbox messages in {stopwatch.ElapsedMilliseconds} ms [job id: '{jobId}'].");

                await Task.Delay((int)_interval.TotalMilliseconds, cancellationToken);
            }
        }

        private async Task SendOutboxMessagesAsync(CancellationToken cancellationToken) 
        {
            using var scope = _scopeFactory.CreateScope();
            
            var outboxManager = scope.ServiceProvider.GetRequiredService<IOutboxManager>();

            var messages = await outboxManager.RetrievePendingMessageAsync(cancellationToken);

            foreach (var message in messages)
            {
                _messageBus.Publish(message.Message, new PublishProperties(_exchange));
                
                await outboxManager.MarkAsSent(message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
