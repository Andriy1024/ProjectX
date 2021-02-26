using Microsoft.EntityFrameworkCore;
using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.DataAccess;
using System;
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
         
        public async Task<ResultOf<ArticleEntity>> GetFullArticleAsync(Expression<Func<ArticleEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return GetResultOf(await DbSet.Include(a => a.Author)
                                          .Include(a => a.Comments)
                                          .FirstOrDefaultAsync(expression, cancellationToken));
        }
    }
}
