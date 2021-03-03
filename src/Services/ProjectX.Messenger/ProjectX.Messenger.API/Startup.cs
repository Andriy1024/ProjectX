using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Infrastructure.Setup;
using ProjectX.Messenger.Application;
using ProjectX.Messenger.Domain;

namespace ProjectX.Messenger.API
{
    public class Startup : BaseStartup<MessangerAppOptions>
    {
        public Startup(IWebHostEnvironment environment, 
                       ILoggerFactory loggerFactory, 
                       IConfiguration configuration) 
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
        }

        private void ConfigureMarten(IServiceCollection services) 
        {
            services.AddMarten(o => 
            {
                o.AutoCreateSchemaObjects = AutoCreate.All;

                o.DatabaseSchemaName = "DocumentStore";
                o.Events.DatabaseSchemaName = "EventStore";


                // This is enough to tell Marten that the User
                // document is persisted and needs schema objects
                //o.Schema.For<User>();

                // Lets Marten know that the event store is active
                o.Events.AddEventType(typeof(ConversationStarted));

                //store.Schema.ApplyAllConfiguredChangesToDatabase();
            })
            .InitializeStore();
        }
    }
}
