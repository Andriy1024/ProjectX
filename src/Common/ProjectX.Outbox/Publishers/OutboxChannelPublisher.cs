using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class OutboxChannelPublisher : BackgroundService
    {
        private readonly OutboxChannel _outboxChannel;
        private readonly OutboxPublisher _outboxPublisher;

        public OutboxChannelPublisher(OutboxChannel outboxChannel, OutboxPublisher outboxPublisher)
        {
            _outboxChannel = outboxChannel;
            _outboxPublisher = outboxPublisher;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var messageId in _outboxChannel.ReadMessagesIdsAsync(stoppingToken))
            {
                try
                {
                    await _outboxPublisher.PublishAsync(m => m.Id == messageId);
                }
                catch
                {
                }
            }
        }
    } 
}
