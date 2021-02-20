using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Blog.Application;
using ProjectX.Blog.Infrastructure.Extensions;
using ProjectX.Blog.Persistence;
using ProjectX.Infrastructure.Extensions;
using ProjectX.Infrastructure.Setup;
using ProjectX.RabbitMq.Configuration;

namespace ProjectX.Blog.API
{
    public sealed class Startup : BaseStartup<BlogOptions>
    {
        public Startup(IWebHostEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration) 
            : base(environment, loggerFactory, configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
              => BaseConfigure(services)
                .AddDbServices<BlogDbContext>(o => o.UseNpgsql(DBConnectionString))
                .AddRabbitMqMessageBus(Configuration)
                .AddTransactinBehaviour()
                .AddRepositories()
                .AddStartupTasks();

        public void Configure(IApplicationBuilder app)
        {
            BaseConfigure(app);
        }
    }
}
