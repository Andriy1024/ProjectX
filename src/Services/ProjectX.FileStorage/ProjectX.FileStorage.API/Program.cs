using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using ProjectX.Infrastructure.Extensions;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.API
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
