using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Common.Infrastructure.DataAccess
{
    public abstract class BaseDbContext<TDbContext> : DbContext, IUnitOfWork, ITransactionActions
        where TDbContext : DbContext
    {
        protected readonly IMediator Mediator;
        protected IDbContextTransaction CurrentTransaction;

        public IDbContextTransaction GetCurrentTransaction() => CurrentTransaction;
        public bool HasActiveTransaction => CurrentTransaction != null;

        public BaseDbContext(DbContextOptions<TDbContext> options) : base(options) { }
        public BaseDbContext(DbContextOptions<TDbContext> options, IMediator mediator) : base(options)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        private EntityEntry<IEntity>[] GetChangedEntities() =>
                ChangeTracker
                .Entries<IEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count > 0)
                .ToArray();

        public virtual async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = GetChangedEntities();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToArray();

            for (int i = 0; i < domainEntities.Length; i++)
                domainEntities[i].Entity.ClearDomainEvents();
 
            await base.SaveChangesAsync(cancellationToken);

            for (int i = 0; i < domainEvents.Length; i++)
                await Mediator.Publish(domainEvents[i]);

            return true;
        }

        public virtual async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (CurrentTransaction != null) return null;

            CurrentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return CurrentTransaction;
        }

        public virtual async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (HasActiveTransaction)
            {
                if (transaction == null) throw new ArgumentNullException(nameof(transaction));
                if (transaction != CurrentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

                try
                {
                    await SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await RollbackTransactionAsync();
                    throw;
                }
                finally
                {
                    if (CurrentTransaction != null)
                    {
                        CurrentTransaction.Dispose();
                        CurrentTransaction = null;
                    }
                }
            }
        }

        public async virtual Task RollbackTransactionAsync()
        {
            try
            {
                await CurrentTransaction?.RollbackAsync();
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }
    }
}
