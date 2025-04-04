using Newtonsoft.Json;

namespace L1.Shared.Json
{
    public interface IJsonService
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string json);
    }

    public class JsonService : IJsonService
    {
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}