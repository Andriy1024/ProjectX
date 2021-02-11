using RabbitMQ.Client;
using System;

namespace ProjectX.MessageBus
{
    public interface IRabbitMqConnectionService : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateChannel();
    }
}
