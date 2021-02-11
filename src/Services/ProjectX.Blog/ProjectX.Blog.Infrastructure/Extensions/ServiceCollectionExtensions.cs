using Microsoft.Extensions.DependencyInjection;
using ProjectX.Blog.Persistence.Setup;
using ProjectX.Core;

namespace ProjectX.Blog.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTasks(this IServiceCollection services)
            => services.AddScoped<IStartupTask, DbStartupTask>();
    }
}
