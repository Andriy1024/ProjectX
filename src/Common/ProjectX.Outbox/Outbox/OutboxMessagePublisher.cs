using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using ProjectX.RabbitMq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class OutboxMessagePublisher : IHostedService, IDisposable
    {
        private readonly IRabbitMqPublisher _messageBus;
        private readonly ILogger<OutboxMessagePublisher> _logger;
        private readonly OutboxOptions _options;
        private readonly Exchange.Name _exchange;
        private readonly TimeSpan _interval;
        private readonly IJsonSerializer _serializer;
        private readonly IServiceScopeFactory _scopeFactory;

        private Timer _timer;

        public OutboxMessagePublisher(IRabbitMqPublisher messageBus,
            IOptions<OutboxOptions> options,
            ILogger<OutboxMessagePublisher> logger,
            IJsonSerializer serializer,
            IServiceScopeFactory scopeFactory)
        {
            _messageBus = messageBus;
            _options = options.Value;
            _exchange = _options.Exchange;
            _interval = TimeSpan.FromMilliseconds(_options.IntervalMilliseconds);
            _logger = logger;
            _serializer = serializer;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(OutboxMessagePublisher)} is running.");

            _timer = new Timer(SendOutboxMessagesAsync, null, TimeSpan.FromSeconds(5), _interval);

            return Task.CompletedTask;
        }

        private async void SendOutboxMessagesAsync(object state) 
        {
            var jobId = Guid.NewGuid().ToString("N");

            _logger.LogTrace($"Started sending outbox messages... [job id: '{jobId}']");
            
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();

            var messages = await dbContext.OutboxMessages.Where(m => !m.SentAt.HasValue).ToArrayAsync();

            for (int i = 0; i < messages.Length; i++)
            {
                var message = messages[i];
                message.Type = Type.GetType(message.MessageType);
                message.Message = _serializer.Deserialize(message.SerializedMessage, message.Type) as IIntegrationEvent;
            }

            foreach (var message in messages)
            {
                _messageBus.Publish(message.Message, p => p.Exchange.Name = _exchange);

                message.SentAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
            }

            stopwatch.Stop();

            _logger.LogTrace($"Finished sending outbox messages in {stopwatch.ElapsedMilliseconds} ms [job id: '{jobId}'].");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(OutboxMessagePublisher)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
