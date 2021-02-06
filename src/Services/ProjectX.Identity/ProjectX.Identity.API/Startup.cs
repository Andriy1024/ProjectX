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
            services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(DBConnectionString))
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
                    .AddHostedService<SessionCleanupWorker>();

            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
            app.UseIdentityServer();
        }
    }
}
