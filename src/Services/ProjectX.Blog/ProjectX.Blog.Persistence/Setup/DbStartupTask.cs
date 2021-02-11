using Microsoft.EntityFrameworkCore;
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
        }
    }
}
