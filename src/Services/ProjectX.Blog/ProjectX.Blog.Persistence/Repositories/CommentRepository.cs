using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using ProjectX.Infrastructure.DataAccess;

namespace ProjectX.Blog.Persistence
{
    public sealed class CommentRepository : Repository<CommentEntity>, ICommentRepository
    {
        public override IUnitOfWork UnitOfWork { get; }

        public CommentRepository(BlogDbContext dbContext) 
            : base(dbContext, notFound: ErrorCode.CommentNotFound)
        {
            UnitOfWork = dbContext;
        }
    }
}
