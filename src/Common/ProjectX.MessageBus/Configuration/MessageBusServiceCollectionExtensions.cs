using Microsoft.Extensions.DependencyInjection;
using ProjectX.RabbitMq.Implementations;
using Microsoft.Extensions.Configuration;
using ProjectX.RabbitMq.Publisher;
using ProjectX.RabbitMq.Subscriber;

namespace ProjectX.RabbitMq.Configuration
{
    public static class MessageBusServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMessageBus(this IServiceCollection services, IConfiguration configuration)
               => services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"))
                          .AddSingleton<IRabbitMqSubscriber, RabbitMqSubscriber>()
                          .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
                          .AddSingleton<IMessageSerializer, DefaultMessageSerializer>()
                          .AddSingleton<IMessageDispatcher, MessageDispatcher>()
                          .AddSingleton<IRabbitMqConnectionService, RabbitMqConnectionService>();
    }
}
