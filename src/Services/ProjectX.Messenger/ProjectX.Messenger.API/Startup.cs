using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Infrastructure.Setup;
using ProjectX.Messenger.Application;
using ProjectX.Messenger.Infrastructure.Extensions;
using ProjectX.Messenger.Persistence;
using ProjectX.RabbitMq.Configuration;
using ProjectX.Realtime.Setup;

namespace ProjectX.Messenger.API
{
    public sealed class Startup : BaseStartup<MessangerAppOptions>
    {
        public Startup(IWebHostEnvironment environment, 
                       ILoggerFactory loggerFactory, 
                       IConfiguration configuration) 
            : base(environment, loggerFactory, configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
                 => BaseConfigure(services)
                   .AddMarten(DBConnectionString)
                   .AddScoped<IEventStore, MartenEventStore>()
                   .AddRealtimeServices()
                   .AddRabbitMqMessageBus(Configuration)
                   .AddStartupTasks();

        public void Configure(IApplicationBuilder app) => BaseConfigure(app); 
    }
}
