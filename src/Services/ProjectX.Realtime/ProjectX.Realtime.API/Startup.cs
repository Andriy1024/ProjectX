using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Infrastructure.Setup;
using ProjectX.RabbitMq.Configuration;
using ProjectX.Realtime.Application;
using ProjectX.Realtime.Application.Setup;
using ProjectX.Realtime.Infrastructure;
using ProjectX.Realtime.Infrastructure.IntegrationEventHandlers;
using WebSocketMiddleware = ProjectX.Realtime.Infrastructure.WebSocketMiddleware;

namespace ProjectX.Realtime.API
{
    public sealed class Startup : BaseStartup<RealtimeAppOptions>
    {
        public Startup(IWebHostEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration) 
            : base(environment, loggerFactory, configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
                 => BaseConfigure(services)
                   .AddSingleton<WebSocketAuthenticationManager>()
                   .AddSingleton<WebSocketManager>()
                   .AddTransient<IIntegrationEventHandler<RealtimeIntegrationEvent>, RealtimeIntegrationEventHandler>()
                   .AddScoped<IStartupTask, MessageBusStartupTask>()
                   .AddRabbitMqMessageBus(Configuration);
        
        public void Configure(IApplicationBuilder app)
                 => BaseConfigure(app)
                   .UseWebSockets()
                   .UseMiddleware<WebSocketMiddleware>();
    }
}
