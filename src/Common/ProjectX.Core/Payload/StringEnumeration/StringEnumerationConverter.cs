using System;
using System.Text.Json;

namespace ProjectX.Core
{
    public class StringEnumerationConverter<T> : System.Text.Json.Serialization.JsonConverter<T>
        where T : StringEnumeration
    {
        public override T Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return StringEnumeration.FindValue<T>(value);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }

    public class StringEnumerationNullableConverter<T> : System.Text.Json.Serialization.JsonConverter<T>
           where T : StringEnumeration
    {
        public override T Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null)
                return null;

            return StringEnumeration.FindValue<T>(value);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.Value);
        }
    }
}
