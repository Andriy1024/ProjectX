using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.DataAccess;

namespace ProjectX.Blog.Persistence
{
    public sealed class CommentRepository : Repository<CommentEntity>, ICommentRepository
    {
        public CommentRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork, notFound: ErrorCode.CommentNotFound)
        {
        }
    }
}
