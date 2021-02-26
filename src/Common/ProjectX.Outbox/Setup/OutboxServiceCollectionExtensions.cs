using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core;
using ProjectX.DataAccess;
using System;

namespace ProjectX.Outbox
{
    public static class OutboxServiceCollectionExtensions
    {
        public static IServiceCollection AddOutboxMessageServices(this IServiceCollection services, IMvcBuilder mvc, IConfiguration configuration, Action<DbContextOptionsBuilder> dbContextOptions)
        {
            OutboxOptions.Validate(configuration.GetSection(nameof(OutboxOptions)).Get<OutboxOptions>());

            return mvc.AddApplicationPart(typeof(OutboxController).Assembly).Services
                      .Configure<OutboxOptions>(configuration.GetSection(nameof(OutboxOptions)))
                      .AddScoped<IStartupTask, OutboxStartupTask>()
                      .AddDbContext<OutboxDbContext>(dbContextOptions)
                      .AddScoped<IOutboxTransaction, OutboxTransaction>()
                      .AddTransient(typeof(IPipelineBehavior<,>), typeof(InboxMessageBehaviour<,>))
                      .AddTransient<INotificationHandler<TransactionCommitedEvent>, TransactionCommitedOutboxHandler>()
                      .AddSingleton<OutboxChannel>()
                      .AddSingleton<OutboxPublisher>()
                      .AddHostedService<OutboxChannelPublisher>()
                      .AddHostedService<OutboxFallbackPublisher>();
        }
    }
}
