using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.DataAccess;

namespace ProjectX.Blog.Persistence
{
    public sealed class AuthorRepository : Repository<AuthorEntity>, IAuthorRepository
    {
        public AuthorRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, notFound: ErrorCode.AuthorNotFound)
        {
        }
    }
}
