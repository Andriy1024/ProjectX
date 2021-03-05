using Microsoft.EntityFrameworkCore;
using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.Core.SeedWork;
using ProjectX.DataAccess;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Persistence
{
    public sealed class ArticleRepository : Repository<ArticleEntity>, IArticleRepository
    {
        public ArticleRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork, notFound: ErrorCode.ArticleNotFound)
        {
        }

        public async Task<IPaginatedResponse<ArticleEntity[]>> GetArticlesWithAuthorAsync(Expression<Func<ArticleEntity, bool>> expression, IPaginationOptions pagination, IOrderingOptions ordering, CancellationToken cancellationToken = default)
        {
            var articles = await DbSet.Include(a => a.Author)
                                      .Where(expression)
                                      .WithOrdering(ordering)
                                      .WithPagination(pagination)
                                      .ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(articles, await CountAsync(expression, articles, pagination, cancellationToken));
        }

        public async Task<ResultOf<ArticleEntity>> GetFullArticleAsync(Expression<Func<ArticleEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return GetResultOf(await DbSet.Include(a => a.Author)
                                          .Include(a => a.Comments)
                                          .FirstOrDefaultAsync(expression, cancellationToken));
        }
    }
}
