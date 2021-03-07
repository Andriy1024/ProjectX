using Marten;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core;
using ProjectX.Messenger.Application.Views;
using ProjectX.Messenger.Domain;
using ProjectX.Messenger.Infrastructure.Setup;
using ProjectX.Messenger.Persistence;

namespace ProjectX.Messenger.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMarten(this IServiceCollection services, string connectionString) 
        {
            services.AddMarten(o =>
            {
                o.Connection(connectionString);
                o.AutoCreateSchemaObjects = AutoCreate.All;
                o.DatabaseSchemaName = "DocumentStore";
                o.Events.DatabaseSchemaName = "EventStore";

                // This is enough to tell Marten that the ConversationView
                // document is persisted and needs schema objects
                o.Schema.For<ConversationView>();
                o.Events.InlineProjections.Add(new ConversationViewProjection());

                // Lets Marten know that the event store is active
                o.Events.AddEventType(typeof(ConversationStarted));
                o.Events.AddEventType(typeof(MessageCreated));
                o.Events.AddEventType(typeof(MessageDeleted));
                o.Events.AddEventType(typeof(MessageUpdated));

                o.Events.StreamIdentity = StreamIdentity.AsString;
            });

            return services;
        }

        public static IServiceCollection AddStartupTasks(this IServiceCollection services)
            => services.AddScoped<IStartupTask, MessageBusStartupTask>()
                       .AddScoped<IStartupTask, DataBaseStartupTask>();
    }
}
