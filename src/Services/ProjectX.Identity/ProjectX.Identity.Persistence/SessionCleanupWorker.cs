using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Persistence
{
    public sealed class SessionCleanupWorker : IHostedService, IDisposable
    {
        Timer _timer;
        readonly IServiceProvider _serviceProvider;
        readonly ILogger<SessionCleanupWorker> _logger;

        public SessionCleanupWorker(IServiceProvider serviceProvider, ILogger<SessionCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(SessionCleanupWorker)} is running.");

            _timer = new Timer(DeleteExpiredSessionsAsync, null, TimeSpan.FromMinutes(1), TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(DeleteExpiredSessionsAsync)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();

        private async void DeleteExpiredSessionsAsync(object state)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                    var sessions = await dbContext.Sessions
                            .Where(s => s.Lifetime.RefreshTokenExpiresAt <= DateTime.UtcNow)
                            .ToArrayAsync();

                    foreach (var session in sessions)
                    {
                        dbContext.Sessions.Remove(session);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(SessionCleanupWorker)} Message:{e.Message}, {e.InnerException}.");
            }
        }
    }
}
