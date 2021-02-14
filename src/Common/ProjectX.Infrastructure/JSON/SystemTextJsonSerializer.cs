using ProjectX.Core.JSON;
using System.Text.Json;

namespace ProjectX.Infrastructure.JSON
{
    public class SystemTextJsonSerializer : ISystemTextJsonSerializer
    {
        readonly JsonSerializerOptions _serializationOptions;

        public SystemTextJsonSerializer()
        {
            _serializationOptions = SerializationOptionsBuilder
                                        .DefaultOptions()
                                        .AddNumberConverters()
                                        .AddJsonNonStringKeyDictionaryConverterFactory();
        }

        public TOut Deserialize<TOut>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TOut>(json, _serializationOptions);
        }

        public string Serialize<TIn>(TIn item)
        {
            return System.Text.Json.JsonSerializer.Serialize(item, item.GetType(), _serializationOptions);
        }

        public TOut Deserialize<TOut>(byte[] json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TOut>(json, _serializationOptions);
        }

        public byte[] SerializeToBytes<TIn>(TIn item)
        {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(item, item.GetType(), _serializationOptions);
        }
    }
}
