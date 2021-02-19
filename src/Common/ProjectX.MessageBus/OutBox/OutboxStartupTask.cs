using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using ProjectX.MessageBus.OutBox;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    //public class OutboxStartupTask : IStartupTask
    //{
    //    private readonly OutboxDbContext _dbContext;

    //    public OutboxStartupTask(OutboxDbContext dbContext)
    //    {
    //        _dbContext = dbContext;
    //    }

    //    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    //    {
    //        _dbContext.Database.Migrate();
    //    }
    //}
}
