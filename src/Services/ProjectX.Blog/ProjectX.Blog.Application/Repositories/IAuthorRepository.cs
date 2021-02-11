using ProjectX.Blog.Domain;
using ProjectX.Core.DataAccess;

namespace ProjectX.Blog.Application
{
    public interface IAuthorRepository : IRepository<AuthorEntity>
    {
    }
}
