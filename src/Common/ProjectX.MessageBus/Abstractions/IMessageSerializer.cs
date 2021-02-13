using ProjectX.Core.JSON;
using System;

namespace ProjectX.MessageBus
{
    public interface IMessageSerializer : ISystemTextJsonSerializer
    {
        object Deserialize(ReadOnlySpan<byte> obj, Type type);
    }
}
