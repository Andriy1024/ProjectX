using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using ProjectX.Identity.Domain;
using ProjectX.Identity.Persistence.EntityConfigurations;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Persistence
{
    public sealed class IdentityDbContext : IdentityDbContext<UserEntity, RoleEntity, long, IdentityUserClaim<long>,
            UserRoleEntity, IdentityUserLogin<long>,
            IdentityRoleClaim<long>, IdentityUserToken<long>>,
            ITransactionActions
    {
        public override DbSet<UserEntity> Users { get; set; }
        public override DbSet<RoleEntity> Roles { get; set; }
        public override DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<SessionEntity> Sessions { get; set; }
        public bool HasActiveTransaction => _currentTransaction != null;

        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;
        
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
                                .Entries<IEntity>()
                                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count > 0)
                                .ToArray();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToArray();

            for (int i = 0; i < domainEntities.Length; i++)
                domainEntities[i].Entity.ClearDomainEvents();

            var result = await base.SaveChangesAsync(cancellationToken);

            for (int i = 0; i < domainEvents.Length; i++)
                await _mediator.Publish(domainEvents[i]);

            return result;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (HasActiveTransaction)
            {
                if (transaction == null) throw new ArgumentNullException(nameof(transaction));
                if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

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
                    if (_currentTransaction != null)
                    {
                        _currentTransaction.Dispose();
                        _currentTransaction = null;
                    }
                }
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

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
    }
}
