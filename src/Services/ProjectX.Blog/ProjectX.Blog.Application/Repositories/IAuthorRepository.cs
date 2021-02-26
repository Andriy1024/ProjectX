using ProjectX.Blog.Domain;
using ProjectX.DataAccess;

namespace ProjectX.Blog.Application
{
    public interface IAuthorRepository : IRepository<AuthorEntity>
    {
    }
}
