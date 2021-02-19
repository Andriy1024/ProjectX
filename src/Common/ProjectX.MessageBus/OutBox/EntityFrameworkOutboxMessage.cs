using Microsoft.EntityFrameworkCore;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    /// <summary>
    /// Outbox Entity Framework Manager
    /// </summary>
    public class EntityFrameworkOutboxMessage<T> : IOutboxManager
        where T: DbContext
    {
        private readonly IJsonSerializer _serializer;
        private readonly T _dbContext;

        public EntityFrameworkOutboxMessage(IJsonSerializer serializer, T dbContext)
        {
            _serializer = serializer;
            _dbContext = dbContext;
        }

        public async Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) 
        {
            var serializedMessage = _serializer.Serialize(integrationEvent);

            var message = new OutboxMessage
                (integrationEvent,  
                serializedMessage: serializedMessage,
                savedAt: DateTime.UtcNow);

            var dbSet = _dbContext.Set<OutboxMessage>();

            await dbSet.AddAsync(message, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<OutboxMessage[]> RetrievePendingMessageAsync(CancellationToken cancellationToken = default) 
        {
            var dbSet = _dbContext.Set<OutboxMessage>();

            var messages = await dbSet.Where(m => !m.SentAt.HasValue).ToArrayAsync(cancellationToken);

            for (int i = 0; i < messages.Length; i++)
            {
                var message = messages[i];
                message.Type = Type.GetType(message.MessageType);
                message.Message = _serializer.Deserialize(message.SerializedMessage, message.Type) as IIntegrationEvent;
            }

            return messages;
        }

        public async Task MarkAsSent(OutboxMessage message) 
        {
            message.SentAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public Task<bool> HasInboxAsync(Guid id) 
        {
            var dbSet = _dbContext.Set<InboxMessage>();
            return dbSet.AnyAsync(m => m.Id == id);
        }

        public async Task AddInboxAsync(IIntegrationEvent integrationEvent) 
        {
            var dbSet = _dbContext.Set<InboxMessage>();

            await dbSet.AddAsync(new InboxMessage() 
            {
                Id = integrationEvent.Id,
                MessageType = integrationEvent.GetType().FullName,
                ProcessedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
