using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Core.DataAccess
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);

        IExecutionStrategy CreateExecutionStrategy();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync(IDbContextTransaction transaction);

        Task RollbackTransactionAsync();

        IDbContextTransaction GetCurrentTransaction();

        IDbConnection GetCurrentConnection();

        bool HasActiveTransaction { get; }

        DbContext DbContext { get; }
    }
}
