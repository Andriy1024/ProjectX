using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class OutboxFallbackPublisher : BackgroundService, IDisposable
    {
        private static readonly TimeSpan MinimumMessageAgeToBatch = TimeSpan.FromSeconds(30);
        private readonly OutboxPublisher _outboxPublisher;
        private readonly ILogger<OutboxFallbackPublisher> _logger;

        public OutboxFallbackPublisher(OutboxPublisher outboxPublisher, ILogger<OutboxFallbackPublisher> logger)
        {
            _outboxPublisher = outboxPublisher;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime minimumMessageAgeToBatch = DateTime.UtcNow.Subtract(MinimumMessageAgeToBatch);
                
                try
                {
                    await _outboxPublisher.PublishAsync(m => !m.SentAt.HasValue && m.SavedAt > minimumMessageAgeToBatch);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
                
                await Task.Delay(MinimumMessageAgeToBatch, stoppingToken);
            }
        }
    }
}
