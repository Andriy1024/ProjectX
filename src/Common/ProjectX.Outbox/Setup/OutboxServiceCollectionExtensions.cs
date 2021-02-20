using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core;
using System;

namespace ProjectX.Outbox
{
    public static class OutboxServiceCollectionExtensions
    {
        public static IServiceCollection AddOutboxMessageServices(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder> optionsAction)
        {
            OutboxOptions.Validate(configuration.GetSection(nameof(OutboxOptions)).Get<OutboxOptions>());

            return services.Configure<OutboxOptions>(configuration.GetSection(nameof(OutboxOptions)))
                           .AddScoped<IOutboxManager, OutboxManager>()
                           .AddHostedService<OutboxMessagePublisher>()
                           .AddScoped(typeof(IPipelineBehavior<,>), typeof(InboxMessageBehaviour<,>))
                           .AddDbContext<OutboxDbContext>(optionsAction)
                           .AddScoped<IStartupTask, OutboxStartupTask>();
        }
    }
}
