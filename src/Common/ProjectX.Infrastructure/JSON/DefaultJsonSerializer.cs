using ProjectX.Core.JSON;
using System;
using System.Text.Json;

namespace ProjectX.Infrastructure.JSON
{
    public class DefaultJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _serializationOptions;

        public DefaultJsonSerializer()
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

        public object Deserialize(string item, Type type)
        {
            return System.Text.Json.JsonSerializer.Deserialize(item, type, _serializationOptions);
        }

        public string Serialize<TIn>(TIn item)
        {
            return System.Text.Json.JsonSerializer.Serialize(item, item.GetType(), _serializationOptions);
        }

        public string Serialize(object item, Type type)
        {
            return System.Text.Json.JsonSerializer.Serialize(item, type, _serializationOptions);
        }
    }
}
