using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class OutboxStartupTask : IStartupTask
    {
        private readonly OutboxDbContext _dbContext;

        public OutboxStartupTask(OutboxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.Database.MigrateAsync();
        }
    }
}
