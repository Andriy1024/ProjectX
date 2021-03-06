using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Infrastructure.Setup;
using ProjectX.Realtime.Application;

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
        }

        public void Configure(IApplicationBuilder app)
        {
            BaseConfigure(app);
            app.UseWebSockets();
            app.UseMiddleware<ProjectX.Realtime.Infrastructure.WebSocket.WebSocketMiddleware>();
        }
    }
}
