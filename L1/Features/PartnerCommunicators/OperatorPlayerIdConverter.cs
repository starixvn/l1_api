using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace L1.Features.PartnerCommunicators
{
    public class OperatorPlayerIdConverter : JsonConverter<dynamic>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int intValue))
            {
                return intValue;
            }

            throw new JsonException("PlayerId must be a string or int.");
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is int intValue)
            {
                writer.WriteNumberValue(intValue);
            }
            else if (value is string stringValue)
            {
                writer.WriteStringValue(stringValue);
            }
            else
            {
                throw new JsonException("PlayerId must be a string or int.");
            }
        }
    }
}