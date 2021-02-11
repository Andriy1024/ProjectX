using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using ProjectX.Infrastructure.DataAccess;

namespace ProjectX.Blog.Persistence
{
    public sealed class AuthorRepository : Repository<AuthorEntity>, IAuthorRepository
    {
        public override IUnitOfWork UnitOfWork { get; }

        public AuthorRepository(BlogDbContext dbContext)
            : base(dbContext, notFound: ErrorCode.AuthorNotFound)
        {
            UnitOfWork = dbContext;
        }
    }
}
