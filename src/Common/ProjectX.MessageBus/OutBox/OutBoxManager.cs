using Microsoft.EntityFrameworkCore;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.OutBox
{
    public class OutboxManager
    {
        readonly IJsonSerializer _serializer;
        readonly DbContext _dbContext;

        public OutboxManager(IJsonSerializer serializer, DbContext dbContext)
        {
            _serializer = serializer;
            _dbContext = dbContext;
        }

        public async Task AddAsync(IIntegrationEvent integrationEvent) 
        {
            var serializedMessage = _serializer.Serialize(integrationEvent);

            var message = new OutboxMessage
                (integrationEvent,  
                serializedMessage: serializedMessage,
                savedAt: DateTime.UtcNow);

            var dbSet = _dbContext.Set<OutboxMessage>();

            await dbSet.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OutboxMessage[]> RetrieveMessagePendingToPublishAsync() 
        {
            var dbSet = _dbContext.Set<OutboxMessage>();

            var messages = await dbSet.Where(m => !m.SentAt.HasValue).ToArrayAsync();

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


        public async Task<bool> HasInbox(Guid id) 
        {
            var dbSet = _dbContext.Set<InboxMessage>();
            return dbSet.Any(m => m.Id == id);
        }

        public async Task AddInbox(Guid id, string messageType) 
        {
            var dbSet = _dbContext.Set<InboxMessage>();

            await dbSet.AddAsync(new InboxMessage() 
            {
                Id = id,
                MessageType = messageType,
                ProcessedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
