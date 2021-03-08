using ProjectX.Core;
using ProjectX.RabbitMq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Application.Setup
{
    public sealed class MessageBusStartupTask : IStartupTask
    {
        private readonly IRabbitMqSubscriber _messageBus;

        public MessageBusStartupTask(IRabbitMqSubscriber messageBus)
        {
            _messageBus = messageBus;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _messageBus.Subscribe<RealtimeIntegrationEvent>(o => 
            {
                o.Exchange.Name = Exchange.Name.Realtime;
                o.Exchange.Type = Exchange.Type.Fanout;
                o.Consumer.Autoack = false;
                o.Consumer.RetryOnFailure = true;
            });

            return Task.CompletedTask;
        }
    }
}
