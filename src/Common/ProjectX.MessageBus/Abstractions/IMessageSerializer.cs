using System;

namespace ProjectX.MessageBus
{
    public interface IMessageSerializer 
    {
        object Deserialize(ReadOnlySpan<byte> obj, System.Type type);

        T Deserialize<T>(ReadOnlySpan<byte> obj);

        byte[] SerializeToBytes(object item, System.Type inputType);
    }
}
