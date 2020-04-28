using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Shared.Constants;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.GameServerCommunicators
{
    public interface IGameServerService
    {
        Task<GameServerResponse<T>> Post<T>(string apiEndpoint, GameServerRequest request) where T : GameServerResult, new();

        Task PostAndForget(string apiEndpoint, GameServerRequest request);
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
            var response = await Post(apiEndpoint, request);

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
                $"Request to Saba Club's {apiEndpoint} failed at {DateTimeOffset.UtcNow} - {failureResponse.Result.ErrorDescription}",
                SentryLevel.Warning);

            return failureResponse;
        }

        public async Task PostAndForget(string apiEndpoint, GameServerRequest request)
        {
            try
            {
                await Post(apiEndpoint, request);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage(ex.ToString(), SentryLevel.Error);
            }
        }

        private Task<HttpResponseMessage> Post(string apiEndpoint, GameServerRequest request)
        {
            const string defaultMediaType = "application/json";

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, defaultMediaType);

            return httpClient.PostAsync(apiEndpoint, content);
        }

        private static async Task<GameServerResponse<T>> CreateErrorResponse<T>(HttpResponseMessage response) where T : GameServerResult, new()
          => new GameServerResponse<T>
          {
              TimeStamp = DateTimeOffset.UtcNow,
              Result = new T
              {
                  ErrorCode = ErrorCodes.InternalError,
                  ErrorDescription = $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}"
              }
          };
    }
}