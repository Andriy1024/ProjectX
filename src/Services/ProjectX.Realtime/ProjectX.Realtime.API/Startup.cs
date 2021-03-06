using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Infrastructure.Setup;
using ProjectX.Realtime.Application;
using ProjectX.Realtime.Infrastructure;
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
        {
            BaseConfigure(services);
            services.AddSingleton<WebSocketAuthenticationManager>();
            services.AddSingleton<WebSocketConnectionManager>();
        }

        public void Configure(IApplicationBuilder app)
        {
            BaseConfigure(app);
            app.UseWebSockets();
            app.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
