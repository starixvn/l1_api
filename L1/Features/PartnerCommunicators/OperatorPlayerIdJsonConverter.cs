using System;
using Newtonsoft.Json;

namespace L1.Features.PartnerCommunicators
{
    public class OperatorPlayerIdJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return reader.Value.ToString();
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                var value = Convert.ToInt64(reader.Value);
                if (value <= int.MaxValue && value >= int.MinValue)
                {
                    return (int)value;
                }
                return value.ToString();
            }

            throw new JsonSerializationException("PlayerId must be a string or integer.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (value is int intValue)
            {
                writer.WriteValue(intValue);
                return;
            }

            if (value is string)
            {
                writer.WriteValue(value);
                return;
            }

            throw new JsonSerializationException("PlayerId must be a string or integer.");
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}