using System;

namespace ProjectX.RabbitMq
{
    public interface IMessageSerializer 
    {
        T Deserialize<T>(ReadOnlySpan<byte> obj);

        byte[] SerializeToBytes(object item, System.Type inputType);
    }
}
