using System;

namespace ProjectX.MessageBus
{
    public interface IMessageSerializer 
    {
        object Deserialize(ReadOnlySpan<byte> obj, Type type);

        byte[] SerializeToBytes(object item, Type inputType);
    }
}
