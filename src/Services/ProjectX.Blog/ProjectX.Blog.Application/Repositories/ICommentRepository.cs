using ProjectX.Blog.Domain;
using ProjectX.DataAccess;

namespace ProjectX.Blog.Application
{
    public interface ICommentRepository : IRepository<CommentEntity>
    {
    }
}
