using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.Persistence.Database.Abstractions
{
    public interface IMongoRepository<TEntity, TIdentifiable>
        where TEntity : IIdentifiable<TIdentifiable>
    {
        public IMongoCollection<TEntity> Collection { get; }
        Task<TEntity> GetAsync(TIdentifiable id, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query, CancellationToken cancellationToken = default) where TQuery : IPagedQuery;
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task DeleteAsync(TIdentifiable id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
