using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<GameServerResponse<T>>(responseContent);
        }
    }
}