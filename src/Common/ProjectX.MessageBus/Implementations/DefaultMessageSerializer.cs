using ProjectX.Core.JSON;
using ProjectX.Core.Realtime;
using System;
using System.Text.Json;

namespace ProjectX.MessageBus.Implementations
{
    public class DefaultMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public DefaultMessageSerializer()
        {
            _serializerOptions = SerializationOptionsBuilder
                                    .DefaultOptions()
                                    .AddJsonNonStringKeyDictionaryConverterFactory()
                                    .AddConverter(new RealtimeMessageConverter());
        }

        public object Deserialize(ReadOnlySpan<byte> obj, Type type)
        {
            return System.Text.Json.JsonSerializer.Deserialize(obj, type, _serializerOptions);
        }

        public byte[] SerializeToBytes(object item, Type inputType)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(item, inputType, _serializerOptions);
        }
    }
}
