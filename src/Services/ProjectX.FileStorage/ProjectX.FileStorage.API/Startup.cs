using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.FileStorage.Application;
using ProjectX.FileStorage.Persistence.FileStorage.Setup;
using ProjectX.Infrastructure.Setup;

namespace ProjectX.FileStorage.API
{
    public class Startup : BaseStartup<FileStorageAppOptions>
    {
        public Startup(IWebHostEnvironment environment, 
                       ILoggerFactory loggerFactory, 
                       IConfiguration configuration) 
                     : base(environment, loggerFactory, configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services) 
                 => BaseConfigure(services)
                   .AddFileStorage();

        public void Configure(IApplicationBuilder app) => BaseConfigure(app);
    }
}
