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
        public static IServiceCollection AddOutboxMessageServices(this IServiceCollection services, IMvcBuilder mvc, IConfiguration configuration, Action<DbContextOptionsBuilder> optionsAction)
        {
            OutboxOptions.Validate(configuration.GetSection(nameof(OutboxOptions)).Get<OutboxOptions>());

            return mvc.AddApplicationPart(typeof(OutboxController).Assembly).Services
                      .Configure<OutboxOptions>(configuration.GetSection(nameof(OutboxOptions)))
                      .AddScoped<IOutboxManager, OutboxManager>()
                      .AddHostedService<OutboxMessagePublisher>()
                      .AddTransient(typeof(IPipelineBehavior<,>), typeof(InboxMessageBehaviour<,>))
                      .AddDbContext<OutboxDbContext>(optionsAction)
                      .AddScoped<IStartupTask, OutboxStartupTask>();
        }
    }
}
