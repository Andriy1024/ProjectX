using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectX.Core;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.DataAccess
{
    public class UnitOfWork<T> : IUnitOfWork
        where T : DbContext
    {
        public DbContext DbContext { get; }

        protected readonly IMediator Mediator;

        public UnitOfWork(T dbContext, IMediator mediator)
        {
            DbContext = dbContext;
            Mediator = mediator;
        }

        private IDbContextTransaction _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public IDbContextTransaction GetCurrentTransaction() 
        {
            if (_currentTransaction == null) throw new InvalidOperationException("UnitOfWork has no active transaction.");

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (HasActiveTransaction)
            {
                if (transaction == null) throw new ArgumentNullException(nameof(transaction));
                if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current.");

                try
                {
                    await DbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await RollbackTransactionAsync();
                    throw;
                }
                finally
                {
                    if (_currentTransaction != null)
                    {
                        _currentTransaction.Dispose();
                        _currentTransaction = null;
                    }
                }

                await Mediator.Publish(new TransactionCommitedEvent(this));
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return DbContext.Database.CreateExecutionStrategy();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) throw new InvalidOperationException("UnitOfWork already has active transaction.");

            _currentTransaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        public IDbConnection GetCurrentConnection() 
        {
            return DbContext.Database.GetDbConnection();
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = DbContext
                                .ChangeTracker
                                .Entries<IEntity>()
                                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count > 0)
                                .ToArray();

            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToArray();

            for (int i = 0; i < domainEntities.Length; i++)
                domainEntities[i].Entity.ClearDomainEvents();

            await DbContext.SaveChangesAsync(cancellationToken);

            for (int i = 0; i < domainEvents.Length; i++)
                await Mediator.Publish(domainEvents[i], cancellationToken);

            return true;
        }
    }
}
