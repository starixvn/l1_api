using Newtonsoft.Json;

namespace L1.Shared.Helpers
{
    public static class JsonStringConverter
    {
        public static string ToJsonString(object obj)
          => obj == null
          ? "null"
          : JsonConvert.SerializeObject(
              obj,
              new JsonSerializerSettings
              {
                  DateTimeZoneHandling = DateTimeZoneHandling.Utc
              });
    }
}