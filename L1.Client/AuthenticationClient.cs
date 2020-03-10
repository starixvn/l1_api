using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace L1.Client
{
    public interface IAuthenticationClient
    {
        Task<SingleSignOnResult> SingleSignOnAsync(SingleSignOnParams signOnParams);
    }

    public class AuthenticationClient : IAuthenticationClient
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly AuthenticationInfo authenticationInfo;
        private readonly Func<AuthenticationInfo, Task<string>> getAccessTokenFunc;

        public AuthenticationClient(AuthenticationInfo authenticationInfo, Func<AuthenticationInfo, Task<string>> getAccessTokenFunc = null)
        {
            this.authenticationInfo = authenticationInfo;
            this.getAccessTokenFunc = getAccessTokenFunc ?? HttpClientManager.GetAccessToken;
        }

        public async Task<SingleSignOnResult> SingleSignOnAsync(SingleSignOnParams signOnParams)
        {
            var accessToken = await getAccessTokenFunc(authenticationInfo);

            client.SetBearerToken(accessToken);

            var paramContent = new StringContent(JsonConvert.SerializeObject(signOnParams), Encoding.UTF8, "application/json");

            var action = $"{authenticationInfo.ApiUri}signin/test";
            var response = await client.PostAsync(action, paramContent);
            if (!response.IsSuccessStatusCode)
            {
                return SingleSignOnResult.ServerErrorResult;
            }

            var redirectUrl = await response.Content.ReadAsStringAsync();

            return SingleSignOnResult.CreateSuccessResponse(redirectUrl);
        }
    }
}