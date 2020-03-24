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
        private readonly HttpClient client;
        private readonly AuthenticationInfo authenticationInfo;

        public AuthenticationClient(AuthenticationInfo authenticationInfo)
        {
            this.authenticationInfo = authenticationInfo;
            client = HttpClientManager.GetApiClient(authenticationInfo.ApiUri);
        }

        public async Task<SingleSignOnResult> SingleSignOnAsync(SingleSignOnParams signOnParams, Action<string> errorHandler = null)
        {
            var tokenResponse = await HttpClientManager.GetAccessToken(authenticationInfo);

            if (tokenResponse.IsError)
            {
                errorHandler?.Invoke($"{tokenResponse.HttpStatusCode}-{tokenResponse.HttpErrorReason}-{tokenResponse.Raw}");
                return CreateErrorResultFromResponseStatusCode(tokenResponse.HttpStatusCode);
            }

            client.SetBearerToken(tokenResponse.AccessToken);

            var paramContent = new StringContent(JsonConvert.SerializeObject(signOnParams), Encoding.UTF8, "application/json");
            var endpoint = "signin/index";
            var response = await client.PostAsync(endpoint, paramContent);

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