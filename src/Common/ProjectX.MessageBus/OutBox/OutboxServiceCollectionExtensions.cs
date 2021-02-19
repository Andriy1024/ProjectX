using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core;
using ProjectX.MessageBus.OutBox;
using System;

namespace ProjectX.MessageBus.Outbox
{
    //public static class OutboxServiceCollectionExtensions
    //{
    //    public static IServiceCollection AddOutboxMessageServices<T>(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder> optionsAction)
    //        where T : DbContext
    //    {
    //        OutboxOptions.Validate(configuration.GetSection(nameof(OutboxOptions)).Get<OutboxOptions>());

    //        return services.Configure<OutboxOptions>(configuration.GetSection(nameof(OutboxOptions)))
    //                       .AddScoped<IOutboxManager, OutboxManager<T>>()
    //                       .AddHostedService<OutboxMessagePublisher>()
    //                       .AddScoped(typeof(IPipelineBehavior<,>), typeof(InboxMessageBehaviour<,>))
    //                       .AddDbContext<OutboxDbContext>(optionsAction)
    //                       .AddScoped<IStartupTask, OutboxStartupTask>();
    //    }
    //}
}
