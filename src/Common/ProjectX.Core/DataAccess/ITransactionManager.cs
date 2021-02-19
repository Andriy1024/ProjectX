using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace ProjectX.Core.DataAccess
{
    /// <summary>
    /// Service with scoped lifetime in DI container.
    /// </summary>
    public interface ITransactionManager
    {
        IExecutionStrategy CreateExecutionStrategy();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync(IDbContextTransaction transaction);

        Task RollbackTransactionAsync();

        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction { get; }
    }
}
