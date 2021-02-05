using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Common.Infrastructure.Setup;
using ProjectX.Identity.Application;

namespace ProjectX.Identity.API
{
    public class Startup : BaseStartup<IdentityOptions>
    {
        public Startup(IWebHostEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration) 
            : base(environment, loggerFactory, configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        } 
    }
}
