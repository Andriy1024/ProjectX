using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using ProjectX.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.DataAccess
{
    public abstract class Repository<TEntity, TKey> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected DbContext Context;
        protected DbSet<TEntity> DbSet;

        public abstract IUnitOfWork UnitOfWork { get; protected set; }

        public Repository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        #region Get

        public virtual Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => DbSet.AnyAsync(expression, cancellationToken);

        public virtual ValueTask<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default) => DbSet.FindAsync(new object[] { id }, cancellationToken);
        public virtual Task<TEntity> GetByIdAsync(TKey key, CancellationToken cancellationToken) => FindAsync(key, cancellationToken).AsTask();
        public virtual Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default) => DbSet.FirstOrDefaultAsync(cancellationToken);
        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => DbSet.FirstOrDefaultAsync(expression, cancellationToken);

        public virtual Task<TEntity[]> GetAsync(IOrderingOptions ordering = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet;
            if (ordering != null)
                query.WithOrdering(ordering);

            return query.ToArrayAsync(cancellationToken);
        }

        public virtual Task<TEntity[]> GetAsync(Expression<Func<TEntity, bool>> expression, IOrderingOptions ordering = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(expression);
            if (ordering != null)
                query.WithOrdering(ordering);

            return query.ToArrayAsync(cancellationToken);
        }

        public virtual Task<TEntity[]> GetAsNoTrackingAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) => DbSet.AsNoTracking().Where(expression).ToArrayAsync(cancellationToken);

        public async Task<IPaginatedResponse<TEntity[]>> GetAsync(Expression<Func<TEntity, bool>> expression, IPaginationOptions pagination, CancellationToken cancellationToken)
        {
            var entities = await DbSet.Where(expression)
                                      .WithPagination(pagination)
                                      .ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(entities, await CountAsync(expression, entities, pagination, cancellationToken));
        }

        public async Task<IPaginatedResponse<TEntity[]>> GetAsync(Expression<Func<TEntity, bool>> expression, IPaginationOptions pagination, IOrderingOptions ordering = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet.Where(expression);

            if (ordering != null)
                query = query.WithOrdering(ordering);

            var entities = await query.WithPagination(pagination).ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(entities, await CountAsync(expression, entities, pagination, cancellationToken));
        }

        #endregion

        #region Insert
        public virtual async ValueTask InsertAsync(TEntity item, CancellationToken cancellationToken = default) => await DbSet.AddAsync(item, cancellationToken);
        public virtual Task InsertRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default) => DbSet.AddRangeAsync(items, cancellationToken);
        #endregion

        #region Update
        public virtual void AttachRange(IEnumerable<TEntity> items) => DbSet.AttachRange(items);
        public virtual void Attach(TEntity item) => DbSet.Attach(item);
        public virtual void Update(TEntity item) => DbSet.Update(item);
        public virtual void UpdateRange(IEnumerable<TEntity> items) => DbSet.UpdateRange(items);
        #endregion

        #region Remove
        public virtual bool Remove(TEntity item) => DbSet.Remove(item).State == EntityState.Deleted;
        public virtual async Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FindAsync(key, cancellationToken);
            return entity != null && DbSet.Remove(entity).State == EntityState.Deleted;
        }
        public virtual async Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            var entity = await DbSet.FirstOrDefaultAsync(expression, cancellationToken);
            return entity != null && DbSet.Remove(entity).State == EntityState.Deleted;
        }
        public virtual void RemoveRange(IEnumerable<TEntity> items) => DbSet.RemoveRange(items);
        public virtual async Task RemoveRangeAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            var entities = await DbSet.Where(expression).ToArrayAsync(cancellationToken);
            if (entities.Length == 0)
                return;

            DbSet.RemoveRange(entities);
        }
        #endregion

        #region Methods with automapper

        public virtual Task<TOut> FirstOrDefaultAsync<TOut>(Expression<Func<TEntity, bool>> expression, IMapper mapper, CancellationToken cancellationToken = default)
           => DbSet.AsNoTracking()
                   .Where(expression)
                   .ProjectTo<TOut>(mapper.ConfigurationProvider)
                   .FirstOrDefaultAsync(cancellationToken);

        public Task<TOut[]> GetAsync<TOut>(IMapper mapper, IOrderingOptions ordering = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking();
            if (ordering != null)
                query = query.WithOrdering(ordering);

            return query.ProjectTo<TOut>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);
        }

        public Task<TOut[]> GetAsync<TOut>(Expression<Func<TEntity, bool>> expression, IMapper mapper, IOrderingOptions ordering = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking().Where(expression);
            if (ordering != null)
                query = query.WithOrdering(ordering);

            return query.ProjectTo<TOut>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);
        }

        public async virtual Task<IPaginatedResponse<TOut[]>> GetAsync<TOut>(IMapper mapper, IPaginationOptions pagination, IOrderingOptions ordering = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking();

            if (ordering != null)
                query = query.WithOrdering(ordering);

            var entities = await query.WithPagination(pagination)
                                     .ProjectTo<TOut>(mapper.ConfigurationProvider)
                                     .ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(entities, await CountAsync(entities, pagination, cancellationToken));
        }

        public async virtual Task<IPaginatedResponse<TOut[]>> GetAsync<TOut>(Expression<Func<TEntity, bool>> expression, IMapper mapper, IPaginationOptions pagination, IOrderingOptions ordering = null, CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking()
                                      .Where(expression);

            if (ordering != null)
                query = query.WithOrdering(ordering);

            var entities = await query.WithPagination(pagination)
                                     .ProjectTo<TOut>(mapper.ConfigurationProvider)
                                     .ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(entities, await CountAsync(expression, entities, pagination, cancellationToken));
        }

        #endregion

        #region Private
        private ValueTask<int> CountAsync<T>(Expression<Func<TEntity, bool>> expression, T[] entities, IPaginationOptions pagination, CancellationToken cancellationToken)
        {
            return entities.Length < pagination.Take
                      ? new ValueTask<int>(entities.Length)
                      : new ValueTask<int>(DbSet.CountAsync(expression, cancellationToken));
        }

        private ValueTask<int> CountAsync<T>(T[] entities, IPaginationOptions pagination, CancellationToken cancellationToken)
        {
            return entities.Length < pagination.Take
                      ? new ValueTask<int>(entities.Length)
                      : new ValueTask<int>(DbSet.CountAsync(cancellationToken));
        }
        #endregion
    }
}
