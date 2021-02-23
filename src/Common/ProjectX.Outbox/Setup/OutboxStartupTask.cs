using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public class OutboxStartupTask : IStartupTask
    {
        private readonly OutboxDbContext _dbContext;

        public OutboxStartupTask(OutboxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.MigrateAsync();
        }
    }
}
