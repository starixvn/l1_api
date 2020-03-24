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
        Task<EndGameInfoResponse> RetrieveEndGameInfoAsync(EndGameInfoRequest request, Action<string> errorHandler = null);
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

        public async Task<EndGameInfoResponse> RetrieveEndGameInfoAsync(EndGameInfoRequest request, Action<string> errorHandler = null)
        {
            var tokenResponse = await HttpClientManager.GetAccessToken(authenticationInfo);

            client.SetBearerToken(tokenResponse.AccessToken);

            var paramContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var endpoint = "member/retrieveendgameinfo";
            var response = await client.PostAsync(endpoint, paramContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<EndGameInfoResponse>(responseContent);
        }
    }
}