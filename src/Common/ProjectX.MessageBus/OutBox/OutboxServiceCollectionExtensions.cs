using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ProjectX.MessageBus.Outbox
{
    public static class OutboxServiceCollectionExtensions
    {
        public static IServiceCollection AddOutboxMessageServices<T>(this IServiceCollection services, IConfiguration configuration)
            where T : DbContext
        {
            var options = configuration.GetSection(nameof(OutboxOptions)).Get<OutboxOptions>();
            if (options == null || options.Exchange == null)
                throw new ArgumentNullException(nameof(OutboxOptions));

            services.Configure<OutboxOptions>(configuration.GetSection(nameof(OutboxOptions)));
            services.AddScoped<IOutboxManager, EntityFrameworkOutboxMessage<T>>();
            services.AddHostedService<OutboxMessagePublisher>();
            return services;
        }
    }
}
