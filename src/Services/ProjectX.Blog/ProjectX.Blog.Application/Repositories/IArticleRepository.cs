using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.Core.SeedWork;
using ProjectX.DataAccess;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Application
{
    public interface IArticleRepository : IRepository<ArticleEntity>
    {
        public Task<ResultOf<ArticleEntity>> GetFullArticleAsync(Expression<Func<ArticleEntity, bool>> expression, CancellationToken cancellationToken = default);
        public Task<IPaginatedResponse<ArticleEntity[]>> GetArticlesWithAuthorAsync(Expression<Func<ArticleEntity, bool>> expression, IPaginationOptions pagination, IOrderingOptions ordering, CancellationToken cancellationToken = default);
    }
}
