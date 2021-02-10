using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Common.Infrastructure.Setup;
using ProjectX.Identity.Infrastructure;
using ProjectX.Identity.Infrastructure.Extensions;
using IdentityDbContext = ProjectX.Identity.Persistence.IdentityDbContext;
using IdentityOptions = ProjectX.Identity.Application.IdentityOptions;
using UserEntity = ProjectX.Identity.Domain.UserEntity;
using RoleEntity = ProjectX.Identity.Domain.RoleEntity;
using ProjectX.Common.Infrastructure.Extensions;
using System.Reflection;
using ProjectX.Identity.Persistence;
using ProjectX.Redis.Configuration;
using ProjectX.Common.Email;
using ProjectX.Common.Infrastructure.BlackList;

namespace ProjectX.Identity.API
{
    public sealed class Startup : BaseStartup<IdentityOptions>
    {
        public Startup(IWebHostEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration) 
            : base(environment, loggerFactory, configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services) 
                 => BaseConfigure(services)
                   .AddDbContext<IdentityDbContext>(options => options.UseNpgsql(DBConnectionString))
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
                   .AddScopedCache()
                   .AddHostedService<SessionCleanupWorker>()
                   .AddEmailServices(Configuration)
                   .AddRedisServices(Configuration)
                   .AddSessionBlackListService();

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<BlackListMiddleware>(); 
                BaseConfigure(app)
               .UseIdentityServer();
        }
    }
}
