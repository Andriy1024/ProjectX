using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectX.Core.Realtime
{
    public sealed class RealtimeMessageConverter : JsonConverter<IRealtimeMessage>
    {
        public override IRealtimeMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<RealtimeMessage>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, IRealtimeMessage value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Used to send realtime messages with camelCase naming policy
            });
        }
    }
}
