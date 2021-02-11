using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Application
{
    public interface IArticleRepository : IRepository<ArticleEntity>
    {
        public Task<ResultOf<ArticleEntity>> GetArticleWithDependetEntitiesAsync(Expression<Func<ArticleEntity, bool>> expression, CancellationToken cancellationToken = default);
    }
}
