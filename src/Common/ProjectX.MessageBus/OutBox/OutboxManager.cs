using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectX.Core.DataAccess;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using ProjectX.MessageBus.OutBox;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    public class OutboxManager<T> : IOutboxManager
        where T: DbContext
    {
        private readonly IJsonSerializer _serializer;
        private readonly OutboxMessageDbContext _outboxDbContext;
        private readonly IUnitOfWork _unitOfWork;

        public OutboxManager(IJsonSerializer serializer, IUnitOfWork unitOfWork)
        {
            _serializer = serializer;
            _unitOfWork = unitOfWork;

            var connection = _unitOfWork.GetCurrentConnection() as DbConnection;

            if (connection == null) 
            {
                throw new InvalidCastException("Can't cast UnitOfWork connection to DbConnection.");
            }

            _outboxDbContext = new OutboxMessageDbContext(new DbContextOptionsBuilder<OutboxMessageDbContext>()
                                                                .UseNpgsql(connection)
                                                                .Options);
        }

        public async Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) 
        {
            var serializedMessage = _serializer.Serialize(integrationEvent);

            var message = new OutboxMessage
                (integrationEvent,  
                serializedMessage: serializedMessage,
                savedAt: DateTime.UtcNow);

            _outboxDbContext.Database.UseTransaction(_unitOfWork.GetCurrentTransaction().GetDbTransaction());

            await _outboxDbContext.OutboxMessages.AddAsync(message, cancellationToken);
            await _outboxDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsSent(OutboxMessage message) 
        {
            message.SentAt = DateTime.UtcNow;
            await _outboxDbContext.SaveChangesAsync();
        }

        public Task<bool> HasInboxAsync(Guid id) 
        {
            var dbSet = _outboxDbContext.Set<InboxMessage>();
            return dbSet.AnyAsync(m => m.Id == id);
        }

        public async Task AddInboxAsync(IIntegrationEvent integrationEvent) 
        {
            var dbSet = _outboxDbContext.Set<InboxMessage>();

            await dbSet.AddAsync(new InboxMessage() 
            {
                Id = integrationEvent.Id,
                MessageType = integrationEvent.GetType().FullName,
                ProcessedAt = DateTime.UtcNow
            });

            await _outboxDbContext.SaveChangesAsync();
        }
    }
}
