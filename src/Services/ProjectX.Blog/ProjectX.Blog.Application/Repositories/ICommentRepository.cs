using ProjectX.Blog.Domain;
using ProjectX.Core.DataAccess;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Application
{
    public interface ICommentRepository : IRepository<CommentEntity>
    {
        public Task<CommentEntity> GetAuthorRequiredAsync(Expression<Func<CommentEntity, bool>> expression, CancellationToken cancellationToken = default);
    }
}
