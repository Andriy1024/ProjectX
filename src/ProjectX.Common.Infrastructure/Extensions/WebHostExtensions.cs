using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ProjectX.Common.Infrastructure.Extensions
{
    public static class WebHostExtensions
    {
        public static async Task RunWithTasksAsync(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var startupTasks = scope.ServiceProvider.GetServices<IStartupTask>();

                foreach (var startupTask in startupTasks)
                    await startupTask.ExecuteAsync();
            }

            await webHost.RunAsync();
        }
    }
}
