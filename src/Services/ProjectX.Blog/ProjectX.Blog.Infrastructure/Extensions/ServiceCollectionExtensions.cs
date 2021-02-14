using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Blog.Application;
using ProjectX.Blog.Infrastructure.Behaviours;
using ProjectX.Blog.Persistence;
using ProjectX.Blog.Persistence.Setup;
using ProjectX.Core;

namespace ProjectX.Blog.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTasks(this IServiceCollection services)
            => services.AddScoped<IStartupTask, DbStartupTask>();

        public static IServiceCollection AddRepositories(this IServiceCollection services)
            => services.AddScoped<IArticleRepository, ArticleRepository>()
                       .AddScoped<IAuthorRepository, AuthorRepository>()
                       .AddScoped<ICommentRepository, CommentRepository>();

        public static IServiceCollection AddPipelineBehaviours(this IServiceCollection services) =>
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(BlogTransactionBehaviour<,>));
    }
}
