using ProjectX.Blog.Domain;
using ProjectX.Core.DataAccess;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Application
{
    public interface IAuthorRepository : IRepository<AuthorEntity>
    {
        public Task<AuthorEntity> GetAuthorRequiredAsync(Expression<Func<AuthorEntity, bool>> expression, CancellationToken cancellationToken = default);
    }
}
