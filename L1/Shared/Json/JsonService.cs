using System.Text.Json;

namespace L1.Shared.Json
{
    public interface IJsonService
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string json);
    }

    public class JsonService : IJsonService
    {
        private readonly JsonSerializerOptions options;

        public JsonService(JsonSerializerOptions options)
        {
            this.options = options;
        }

        public string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, options);
        }

        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}