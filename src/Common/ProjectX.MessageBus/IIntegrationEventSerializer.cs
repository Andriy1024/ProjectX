using ProjectX.Core.JSON;
using System;

namespace ProjectX.MessageBus
{
    public interface IIntegrationEventSerializer : ISystemTextJsonSerializer
    {
        object Deserialize(byte[] json, Type type);
    }
}
