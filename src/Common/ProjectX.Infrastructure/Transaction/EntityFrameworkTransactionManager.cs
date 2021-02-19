using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectX.Core.DataAccess;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.Transaction
{
    public class EntityFrameworkTransactionManager<T> : ITransactionManager
        where T : DbContext
    {
        private readonly T DbContext;

        private IDbContextTransaction _currentTransaction;

        public EntityFrameworkTransactionManager(T dbContext)
        {
            DbContext = dbContext;
        }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (HasActiveTransaction)
            {
                if (transaction == null) throw new ArgumentNullException(nameof(transaction));
                if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

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
            if (_currentTransaction != null) throw new InvalidOperationException("Transaction manager already has active transaction.");

            _currentTransaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }
    }
}
