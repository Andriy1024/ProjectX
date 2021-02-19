using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectX.Core.DataAccess;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public class OutboxManager : IOutboxManager
    {
        private readonly IJsonSerializer _serializer;
        private readonly OutboxDbContext _outboxDbContext;
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

            _outboxDbContext = new OutboxDbContext(new DbContextOptionsBuilder<OutboxDbContext>()
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

            await _outboxDbContext.OutboxMessages.AddAsync(message, cancellationToken);
            await _outboxDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> HasInboxAsync(Guid id) 
        {
            return _outboxDbContext.InboxMessages.AnyAsync(m => m.Id == id);
        }

        public async Task AddInboxAsync(IIntegrationEvent integrationEvent) 
        {
            _outboxDbContext.Database.UseTransaction(_unitOfWork.GetCurrentTransaction().GetDbTransaction());

            await _outboxDbContext.InboxMessages.AddAsync(new InboxMessage() 
            {
                Id = integrationEvent.Id,
                MessageType = integrationEvent.GetType().FullName,
                ProcessedAt = DateTime.UtcNow
            });

            await _outboxDbContext.SaveChangesAsync();
        }
    }
}
