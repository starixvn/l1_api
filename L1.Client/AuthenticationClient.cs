using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace L1.Client
{
    public interface IAuthenticationClient
    {
        Task<SingleSignOnResult> SingleSignOnAsync(SingleSignOnParams signOnParams, Action<string> errorHandler = null);
    }

    public class AuthenticationClient : IAuthenticationClient
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly AuthenticationInfo authenticationInfo;
        private readonly Func<AuthenticationInfo, Task<TokenResponse>> getAccessTokenFunc;

        public AuthenticationClient(AuthenticationInfo authenticationInfo, Func<AuthenticationInfo, Task<TokenResponse>> getAccessTokenFunc = null)
        {
            this.authenticationInfo = authenticationInfo;
            this.getAccessTokenFunc = getAccessTokenFunc ?? HttpClientManager.GetAccessToken;
        }

        public async Task<SingleSignOnResult> SingleSignOnAsync(SingleSignOnParams signOnParams, Action<string> errorHandler = null)
        {
            var tokenResponse = await getAccessTokenFunc(authenticationInfo);

            if (tokenResponse.IsError)
            {
                errorHandler?.Invoke($"{tokenResponse.HttpStatusCode}-{tokenResponse.HttpErrorReason}-{tokenResponse.Raw}");
                return CreateErrorResultFromResponseStatusCode(tokenResponse.HttpStatusCode);
            }

            client.SetBearerToken(tokenResponse.AccessToken);

            var paramContent = new StringContent(JsonConvert.SerializeObject(signOnParams), Encoding.UTF8, "application/json");

            var action = $"{authenticationInfo.ApiUri}signin/index";
            var response = await client.PostAsync(action, paramContent);
            if (!response.IsSuccessStatusCode)
            {
                errorHandler?.Invoke($"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}");
                return CreateErrorResultFromResponseStatusCode(response.StatusCode);
            }

            var redirectUrl = await response.Content.ReadAsStringAsync();

            return SingleSignOnResult.CreateSuccessResponse(redirectUrl);
        }

        private SingleSignOnResult CreateErrorResultFromResponseStatusCode(HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.Unauthorized)
            {
                return SingleSignOnResult.CreateUnauthorizedErrorResponse();
            }

            if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                return SingleSignOnResult.CreateInvalidDataErrorResponse();
            }

            if (httpStatusCode == HttpStatusCode.NotFound)
            {
                return SingleSignOnResult.CreateServerNotFoundResponse();
            }

            return SingleSignOnResult.CreateServerErrorResponse();
        }
    }
}