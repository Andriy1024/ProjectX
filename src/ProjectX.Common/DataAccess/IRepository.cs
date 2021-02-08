using AutoMapper;
using ProjectX.Common.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Common.DataAccess
{
    public interface IRepository<TEntity>
    {
        #region Methods with automapper
        Task<TOut[]> GetAsync<TOut>(IMapper mapper, IOrderingOptions ordering = null, CancellationToken cancellationToken = default);

        Task<TOut[]> GetAsync<TOut>(Expression<Func<TEntity, bool>> expression, IMapper mapper, IOrderingOptions ordering = null, CancellationToken cancellationToken = default);
        Task<IPaginatedResponse<TOut[]>> GetAsync<TOut>(IMapper mapper, IPaginationOptions pagination, IOrderingOptions ordering = null, CancellationToken cancellationToken = default);
        Task<IPaginatedResponse<TOut[]>> GetAsync<TOut>(Expression<Func<TEntity, bool>> expression, IMapper mapper, IPaginationOptions pagination, IOrderingOptions ordering = null, CancellationToken cancellationToken = default);

        Task<TOut> FirstOrDefaultAsync<TOut>(Expression<Func<TEntity, bool>> expression, IMapper mapper, CancellationToken cancellationToken = default);
        #endregion

        #region Get
        Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        Task<TEntity[]> GetAsync(IOrderingOptions ordering = null, CancellationToken cancellationToken = default);
        Task<TEntity[]> GetAsNoTrackingAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        Task<TEntity[]> GetAsync(Expression<Func<TEntity, bool>> expression, IOrderingOptions ordering = null, CancellationToken cancellationToken = default);
        Task<IPaginatedResponse<TEntity[]>> GetAsync(Expression<Func<TEntity, bool>> expression, IPaginationOptions pagination, CancellationToken cancellationToken = default);
        Task<IPaginatedResponse<TEntity[]>> GetAsync(Expression<Func<TEntity, bool>> expression, IPaginationOptions pagination, IOrderingOptions ordering = null, CancellationToken cancellationToken = default);

        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        #endregion

        #region Insert
        ValueTask InsertAsync(TEntity item, CancellationToken cancellationToken = default);
        Task InsertRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);
        #endregion

        #region Update
        void Update(TEntity item);
        void UpdateRange(IEnumerable<TEntity> items);
        void AttachRange(IEnumerable<TEntity> items);
        void Attach(TEntity item);
        #endregion

        #region Remove
        bool Remove(TEntity item);
        Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        void RemoveRange(IEnumerable<TEntity> items);
        Task RemoveRangeAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        #endregion

        IUnitOfWork UnitOfWork { get; }
    }
}
