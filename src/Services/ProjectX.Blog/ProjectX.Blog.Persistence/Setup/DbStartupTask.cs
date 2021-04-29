using Microsoft.EntityFrameworkCore;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Persistence.Setup
{
    public sealed class DbStartupTask : IStartupTask
    {
        readonly BlogDbContext _dbContext;

        public DbStartupTask(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.MigrateAsync();
            
            if(!await _dbContext.Authors.AnyAsync(a => a.Email == "admin@projectX.com")) 
            {
                _dbContext.Authors.Add(
                    new AuthorEntity(
                        id: 1, 
                        firstName: "Admin",
                        lastName: "Admin",
                        email: "admin@projectX.com"
                    ));

                _dbContext.SaveChanges();
            }
        }
    }
}
