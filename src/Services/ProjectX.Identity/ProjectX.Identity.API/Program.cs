using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using ProjectX.Common.Infrastructure.Extensions;

namespace ProjectX.Identity.API
{
    public class Program
    {
        public static async Task Main(string[] args) =>
            await CreateHostBuilder(args)
                    .Build()
                    .RunWithTasksAsync();

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>();
    }
}
