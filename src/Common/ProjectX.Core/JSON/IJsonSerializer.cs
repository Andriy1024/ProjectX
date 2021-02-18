using System;

namespace ProjectX.Core.JSON
{
    public interface IJsonSerializer
    {
        TOut Deserialize<TOut>(string json);
        string Serialize<TIn>(TIn item);
        string Serialize(object item, Type type);
        object Deserialize(string item, Type type);
    }

    public interface ISystemTextJsonSerializer : IJsonSerializer
    {
        TOut Deserialize<TOut>(byte[] json);
        byte[] SerializeToBytes<TIn>(TIn item);
    }
}
