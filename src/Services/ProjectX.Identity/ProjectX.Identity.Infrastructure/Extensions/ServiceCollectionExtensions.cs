using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core;
using ProjectX.Identity.Domain;
using ProjectX.Identity.Infrastructure.Behaviours;
using ProjectX.Identity.Infrastructure.Managers;
using ProjectX.Identity.Persistence.Startup;

namespace ProjectX.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTasks(this IServiceCollection services) =>
             services.AddTransient<IStartupTask, AspNetIdentityStartupTask>()
                     .AddTransient<IStartupTask, IdentityServerStartupTask>();

        public static IServiceCollection AddIdentityServer4(this IServiceCollection services, string connectionString, string assemblyName) =>
             services.AddIdentityServer()
                     .AddDeveloperSigningCredential()
                     .AddAspNetIdentity<UserEntity>()
                     .AddProfileService<ProfileManager>()
                     .AddResourceOwnerValidator<ResourceOwnerPasswordValidator<UserEntity>>()
                     .AddConfigurationStore(options =>
                     {
                         options.ConfigureDbContext = builder =>
                                builder.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(assemblyName));
                     })
                     //this adds the operational data from DB (codes, tokens, consents)
                     .AddOperationalStore(options =>
                     {
                         options.ConfigureDbContext = builder =>
                                builder.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(assemblyName));
                         
                         // this enables automatic token cleanup. this is optional.
                         options.EnableTokenCleanup = true;
                     })
                     .Services;

        public static IServiceCollection AddPipelineBehaviours(this IServiceCollection services) =>
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdentityTransactionBehaviour<,>));   
    }
}
