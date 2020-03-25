using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace L1.Client
{
    public interface IMemberClient
    {
        Task<EndGameInfoResult> RetrieveEndGameInfoAsync(EndGameInfoRequest request, Action<string> errorHandler = null);
    }

    public class MemberClient : IMemberClient
    {
        private readonly HttpClient client;
        private readonly AuthenticationInfo authenticationInfo;

        public MemberClient(AuthenticationInfo authenticationInfo)
        {
            this.authenticationInfo = authenticationInfo;
            client = HttpClientManager.GetApiClient(authenticationInfo.ApiUri);
        }

        public async Task<EndGameInfoResult> RetrieveEndGameInfoAsync(EndGameInfoRequest request, Action<string> errorHandler = null)
        {
            var paramContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            const string endpoint = "member/retrieveendgameinfo";
            var response = await client.PostAsync(endpoint, paramContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            return new EndGameInfoResult(JsonConvert.DeserializeObject<EndGameInfoData>(responseContent));
        }
    }
}