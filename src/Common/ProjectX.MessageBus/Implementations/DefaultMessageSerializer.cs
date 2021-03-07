using ProjectX.Core.JSON;
using System;
using System.Text.Json;

namespace ProjectX.RabbitMq.Implementations
{
    public class DefaultMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public DefaultMessageSerializer()
        {
            _serializerOptions = SerializationOptionsBuilder
                                    .DefaultOptions()
                                    .AddJsonNonStringKeyDictionaryConverterFactory();
        }

        public T Deserialize<T>(ReadOnlySpan<byte> obj)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(obj, _serializerOptions);

        }

        public byte[] SerializeToBytes(object item, System.Type inputType)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(item, inputType, _serializerOptions);
        }
    }
}
