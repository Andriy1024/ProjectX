using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Infrastructure.Setup;
using ProjectX.Identity.Infrastructure;
using ProjectX.Identity.Infrastructure.Extensions;
using IdentityDbContext = ProjectX.Identity.Persistence.IdentityDbContext;
using IdentityOptions = ProjectX.Identity.Application.IdentityOptions;
using UserEntity = ProjectX.Identity.Domain.UserEntity;
using RoleEntity = ProjectX.Identity.Domain.RoleEntity;
using ProjectX.Infrastructure.Extensions;
using System.Reflection;
using ProjectX.Identity.Persistence;
using ProjectX.Redis.Configuration;
using ProjectX.Infrastructure.BlackList;
using ProjectX.Email;
using ProjectX.RabbitMq.Configuration;
using Microsoft.EntityFrameworkCore;
using ProjectX.Outbox;
using ProjextX.DataAccess.Extensions;

namespace ProjectX.Identity.API
{
    public sealed class Startup : BaseStartup<IdentityOptions>
    {
        public Startup(IWebHostEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration) 
            : base(environment, loggerFactory, configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
           services.AddDbServices<IdentityDbContext>(o => o.UseNpgsql(DBConnectionString))
                   .AddIdentity<UserEntity, RoleEntity>(options =>
                   {
                       options.User.RequireUniqueEmail = true;
                   })
                   .AddRoles<RoleEntity>()
                   .AddEntityFrameworkStores<IdentityDbContext>()
                   .AddUserManager<UserManager>()
                   .AddDefaultTokenProviders()
                   .Services
                   .AddIdentityServer4(DBConnectionString, typeof(IdentityDbContext).GetTypeInfo().Assembly.GetName().Name)
                   .AddStartupTasks()
                   .AddTransactinBehaviour()
                   .AddScopedCache()
                   .AddRabbitMqMessageBus(Configuration)
                   .AddHostedService<SessionCleanupWorker>()
                   .AddEmailServices(Configuration)
                   .AddRedisServices(Configuration)
                   .AddSessionBlackListService();
                    BaseConfigure(services)
                   .AddOutboxMessageServices(MvcBuilder, Configuration, o => o.UseNpgsql(DBConnectionString, sql => sql.MigrationsAssembly(typeof(IdentityDbContext).GetTypeInfo().Assembly.GetName().Name)));
        }
                   
        public void Configure(IApplicationBuilder app)
                => BaseConfigure(app)
                  .UseMiddleware<BlackListMiddleware>()
                  .UseIdentityServer();
    }
}
