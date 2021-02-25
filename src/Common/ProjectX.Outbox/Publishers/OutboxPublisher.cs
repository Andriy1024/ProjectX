using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using ProjectX.RabbitMq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class OutboxPublisher 
    {
        private readonly OutboxOptions _options;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly IJsonSerializer _serializer;
        private readonly ILogger<OutboxPublisher> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Exchange.Name _exchange;

        public OutboxPublisher(IOptions<OutboxOptions> options, 
            IRabbitMqPublisher rabbitMqPublisher,
            IJsonSerializer serializer,
            ILogger<OutboxPublisher> logger, 
            IServiceScopeFactory scopeFactory)
        {
            _options = options.Value;
            _rabbitMqPublisher = rabbitMqPublisher;
            _serializer = serializer;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _exchange = _options.Exchange;
        }

        public async Task PublishAsync(Expression<Func<OutboxMessage, bool>> predicate) 
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();

            var messages = await dbContext.OutboxMessages.Where(predicate).ToArrayAsync();

            for (int i = 0; i < messages.Length; i++)
            {
                var integrationEvent = TryDeserialize(messages[i]);

                if(integrationEvent != null) 
                {
                    _rabbitMqPublisher.Publish(integrationEvent,  p => p.Exchange.Name = _exchange);

                    messages[i].SentAt = DateTime.UtcNow;

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private IIntegrationEvent? TryDeserialize(OutboxMessage outboxMessage) 
        {
            try
            {
                var type = Type.GetType(outboxMessage.MessageType);
                
                return _serializer.Deserialize(outboxMessage.SerializedMessage, type) as IIntegrationEvent;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            return null;
        }
    }
}
