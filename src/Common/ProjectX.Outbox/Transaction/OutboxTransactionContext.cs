using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectX.DataAccess;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.JSON;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class OutboxTransactionContext : IOutboxTransactionContext
    {
        private readonly IJsonSerializer _serializer;
        private readonly OutboxDbContext _outboxDbContext;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// The ids are wrote in the outbox channel when the transaction is committed.
        /// </summary>
        private readonly Queue<Guid> _messageIds;
        private readonly OutboxChannel _outboxChannel;

        public OutboxTransactionContext(IJsonSerializer serializer,
            IUnitOfWork unitOfWork,
            OutboxChannel outboxChannel)
        {
            _serializer = serializer;
            _unitOfWork = unitOfWork;
            _outboxChannel = outboxChannel;

            var connection = _unitOfWork.GetCurrentConnection() as DbConnection;

            if (connection == null) 
            {
                throw new InvalidCastException("Can't cast UnitOfWork connection to DbConnection.");
            }

            _messageIds = new Queue<Guid>();

            _outboxDbContext = new OutboxDbContext(new DbContextOptionsBuilder<OutboxDbContext>()
                                                                .UseNpgsql(connection)
                                                                .Options);
        }

        public async Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) 
        {
            var serializedMessage = _serializer.Serialize(integrationEvent);

            var message = new OutboxMessage(integrationEvent,  
                                            serializedMessage: serializedMessage,
                                            savedAt: DateTime.UtcNow);

            _outboxDbContext.Database.UseTransaction(_unitOfWork.GetCurrentTransaction().GetDbTransaction());

            await _outboxDbContext.OutboxMessages.AddAsync(message, cancellationToken);
            
            await _outboxDbContext.SaveChangesAsync(cancellationToken);
            
            _messageIds.Enqueue(integrationEvent.Id);
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

        public Task OnTransactionCommitedAsync()
        {
            while (_messageIds.TryDequeue(out var messageId)) 
            {
                _outboxChannel.WriteNewMessages(messageId);
            }
                 
            return Task.CompletedTask;
        }
    }
}
