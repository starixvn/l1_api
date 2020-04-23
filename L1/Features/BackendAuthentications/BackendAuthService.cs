using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace L1.Features.BackendAuthentications
{
    public interface IBackendAuthService
    {
        Task<TokenResponse> GetAccessToken();
    }

    public class BackendAuthService : IBackendAuthService
    {
        private readonly HttpClient httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;

        public BackendAuthService(HttpClient httpClient, BackendAuthConfigs backendAuthConfigs)
        {
            this.httpClient = httpClient;
            this.backendAuthConfigs = backendAuthConfigs;
        }

        public async Task<TokenResponse> GetAccessToken()
        {
            var request = new DiscoveryDocumentRequest
            {
                Address = backendAuthConfigs.Address,
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = false
                }
            };

            var disco = await httpClient.GetDiscoveryDocumentAsync(request);

            if (disco.IsError)
            {
                throw new InvalidOperationException(disco.Error);
            }

            return await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = backendAuthConfigs.ClientId,
                ClientSecret = backendAuthConfigs.ClientSecret,
                Scope = backendAuthConfigs.Scope,
            });
        }
    }
}