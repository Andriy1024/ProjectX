using Marten;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Messenger.Application.Views;
using ProjectX.Messenger.Domain;

namespace ProjectX.Messenger.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMarten(this IServiceCollection services, string connectionString) 
        {
            services.AddMarten(o =>
            {
                //o.CreateDatabasesForTenants(c =>
                //{
                //    c.MaintenanceDatabase(DBConnectionString);

                //    c.ForTenant()
                //      .CheckAgainstPgDatabase()
                //      .WithOwner("postgres")
                //      .WithEncoding("UTF-8")
                //      .ConnectionLimit(-1)
                //      .OnDatabaseCreated(c => System.Console.WriteLine("DB CREATED"));
                //});

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
            }).InitializeStore();

            return services;
        }
    }
}
