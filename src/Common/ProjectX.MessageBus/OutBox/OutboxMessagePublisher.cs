using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using ProjectX.Core.Setup;
using ProjectX.MessageBus.OutBox;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    //public class OutboxMessagePublisher : IHostedService
    //{
    //    private readonly IMessageBus _messageBus;
    //    private readonly ILogger<OutboxMessagePublisher> _logger;
    //    private readonly OutboxOptions _options;
    //    private readonly MessageBusExchanges _exchange;
    //    private readonly TimeSpan _interval;
    //    private readonly string _connectionString;
    //    private readonly IJsonSerializer _serializer;

    //    public OutboxMessagePublisher(IMessageBus messageBus,
    //        IOptions<OutboxOptions> options,
    //        IOptions<ConnectionStrings> connectionString,
    //        ILogger<OutboxMessagePublisher> logger,
    //        IJsonSerializer serializer)
    //    {
    //        _messageBus = messageBus;
    //        _options = options.Value;
    //        _connectionString = connectionString.Value.DbConnection;
    //        _exchange = _options.Exchange;
    //        _interval = TimeSpan.FromMilliseconds(_options.IntervalMilliseconds);
    //        _logger = logger;
    //        _serializer = serializer;
    //    }

    //    public async Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        await Task.Delay((int)TimeSpan.FromSeconds(10).TotalMilliseconds, cancellationToken);

    //        while (!cancellationToken.IsCancellationRequested)
    //        {
    //            var jobId = Guid.NewGuid().ToString("N");
                
    //            _logger.LogTrace($"Started sending outbox messages... [job id: '{jobId}']");
                
    //            var stopwatch = new Stopwatch();
                
    //            stopwatch.Start();

    //            try
    //            {
    //                await SendOutboxMessagesAsync(cancellationToken);
    //            }
    //            catch (OperationCanceledException) 
    //            {
    //                break;
    //            }
    //            catch (Exception e)
    //            {
    //                _logger.LogError(e, e.Message);
    //            }

    //            stopwatch.Stop();

    //            _logger.LogTrace($"Finished sending outbox messages in {stopwatch.ElapsedMilliseconds} ms [job id: '{jobId}'].");

    //            await Task.Delay((int)_interval.TotalMilliseconds, cancellationToken);
    //        }
    //    }

    //    private async Task SendOutboxMessagesAsync(CancellationToken cancellationToken) 
    //    {
    //        using var dbContext = new OutboxDbContext(new DbContextOptionsBuilder<OutboxDbContext>()
    //                                                            .UseNpgsql(_connectionString)
    //                                                            .Options);

    //        var messages = await dbContext.OutboxMessages.Where(m => !m.SentAt.HasValue).ToArrayAsync(cancellationToken);

    //        for (int i = 0; i < messages.Length; i++)
    //        {
    //            var message = messages[i];
    //            message.Type = System.Type.GetType(message.MessageType);
    //            message.Message = _serializer.Deserialize(message.SerializedMessage, message.Type) as IIntegrationEvent;
    //        }

    //        foreach (var message in messages)
    //        {
    //            _messageBus.Publish(message.Message, new PublishProperties(_exchange));

    //            message.SentAt = DateTime.UtcNow;

    //            await dbContext.SaveChangesAsync();
    //        }
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        return Task.CompletedTask;
    //    }
    //}
}
