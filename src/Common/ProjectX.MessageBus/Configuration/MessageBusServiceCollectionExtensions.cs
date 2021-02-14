using Microsoft.Extensions.DependencyInjection;
using ProjectX.MessageBus.Implementations;
using Microsoft.Extensions.Configuration;

namespace ProjectX.MessageBus.Configuration
{
    public static class MessageBusServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMessageBus(this IServiceCollection services, IConfiguration configuration)
               => services.Configure<MessageBusConfiguration>(configuration.GetSection("MessageBus"))
                          .AddSingleton<IMessageBus, RabbitMqMessageBus>()
                          .AddSingleton<IMessageSerializer, DefaultMessageSerializer>()
                          .AddSingleton<IMessageDispatcher, MessageDispatcher>()
                          .AddSingleton<IRabbitMqConnectionService, RabbitMqConnectionService>();
    }
}
