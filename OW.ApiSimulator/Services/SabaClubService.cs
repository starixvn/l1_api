using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OW.ApiSimulator.Models.Saba;

namespace OW.ApiSimulator.Services
{
    public interface ISabaClubService
    {
        Task<SabaResponse<EndGameInfoResult>> RetrieveEndGameInfoAsync(EndGameInfoRequest request);
    }

    public class SabaClubService : ISabaClubService
    {
        private readonly HttpClient httpClient;
        private const string defaultMediaType = "application/json";

        public SabaClubService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<SabaResponse<EndGameInfoResult>> RetrieveEndGameInfoAsync(EndGameInfoRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, defaultMediaType);
            var response = await httpClient.PostAsync("member/RetrieveEndGameInfo", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SabaResponse<EndGameInfoResult>>(responseContent);
        }
    }
}