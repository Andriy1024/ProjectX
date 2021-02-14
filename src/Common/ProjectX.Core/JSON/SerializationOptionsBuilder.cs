using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectX.Core.JSON
{
    public static class SerializationOptionsBuilder
    {
        public static JsonSerializerOptions DefaultOptions() =>
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            };

        public static JsonSerializerOptions AddJsonNonStringKeyDictionaryConverterFactory(this JsonSerializerOptions options) 
        {
            options.Converters.Add(GetJsonNonStringKeyDictionaryConverterFactory());
            return options;
        }

        public static JsonConverter GetJsonNonStringKeyDictionaryConverterFactory() =>
            new JsonNonStringKeyDictionaryConverterFactory();

        public static JsonSerializerOptions AddNumberConverters(this JsonSerializerOptions options)
        {
            foreach (var converter in GetNumberConverters())
                options.Converters.Add(converter);

            return options;
        }

        public static IEnumerable<JsonConverter> GetNumberConverters()
        {
            yield return new DoubleToStringConverter();
            yield return new IntToStringConverter();
            yield return new LongToStringConverter();
            yield return new JsonNumberToStringConverter();
            yield return new JsonDoubleToStringConverter();
            yield return new DecimalToStringConverter();
        }

        public static JsonSerializerOptions AddConverter(this JsonSerializerOptions options, JsonConverter converter)
        {
            options.Converters.Add(converter);
            return options;
        }
    }
}
