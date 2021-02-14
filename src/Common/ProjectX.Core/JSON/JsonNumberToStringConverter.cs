using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectX.Core.JSON
{
    public class JsonNumberToStringConverter : JsonConverter<String>
    {
        public override String Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out long number, out int bytesConsumed) && span.Length == bytesConsumed)
                    return number.ToString();

                if (Int64.TryParse(reader.GetString(), out number))
                    return number.ToString();
            }

            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string longValue, JsonSerializerOptions options)
        {
            writer.WriteStringValue(longValue);
        }
    }

    public class JsonDoubleToStringConverter : JsonConverter<String>
    {
        public override String Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out double number, out int bytesConsumed) && span.Length == bytesConsumed)
                    return number.ToString();

                if (Double.TryParse(reader.GetString(), out number))
                    return number.ToString();
            }

            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string longValue, JsonSerializerOptions options)
        {
            writer.WriteStringValue(longValue);
        }
    }
}
