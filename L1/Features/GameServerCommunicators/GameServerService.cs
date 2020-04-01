using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.GameServerCommunicators
{
    public interface IGameServerService
    {
        Task<GameServerResponse<T>> Post<T>(string apiEndpoint, GameServerRequest request) where T : GameServerResult, new();
    }

    public class GameServerService : IGameServerService
    {
        private readonly HttpClient httpClient;

        public GameServerService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<GameServerResponse<T>> Post<T>(string apiEndpoint, GameServerRequest request) where T : GameServerResult, new()
        {
            const string defaultMediaType = "application/json";

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, defaultMediaType);
            var response = await httpClient.PostAsync(apiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return new GameServerResponse<T>
                {
                    TimeStamp = DateTimeOffset.UtcNow,
                    Result = JsonConvert.DeserializeObject<T>(responseContent)
                };
            }

            var failureResponse = await CreateErrorResponse<T>(response);

            SentrySdk.CaptureMessage(
                $"Request to Saba Club failed at {DateTimeOffset.UtcNow} - {failureResponse.Result.ErrorDescription}",
                SentryLevel.Warning);

            return failureResponse;
        }

        private static async Task<GameServerResponse<T>> CreateErrorResponse<T>(HttpResponseMessage response) where T : GameServerResult, new()
          => new GameServerResponse<T>
          {
              TimeStamp = DateTimeOffset.UtcNow,
              Result = new T
              {
                  ErrorDescription = $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}"
              }
          };
    }
}