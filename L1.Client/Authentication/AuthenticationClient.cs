using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            var paramContent = new StringContent(JsonConvert.SerializeObject(signOnParams), Encoding.UTF8, "application/json");
            const string endpoint = "signin/index";
            var response = await client.PostAsync(endpoint, paramContent);

            if (!response.IsSuccessStatusCode)
            {
                errorHandler?.Invoke($"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}");
                return new SingleSignOnResult(Error.CreateErrorFromResponseStatusCode(response.StatusCode));
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var signInResponse = JsonConvert.DeserializeObject<SignInResponse>(responseContent);

            return new SingleSignOnResult(new Uri(signInResponse.RedirectUrl));
        }
    }
}