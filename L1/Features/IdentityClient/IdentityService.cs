using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using L1.Shared.Configurations;

namespace L1.Features.IdentityClient
{
    public interface IIdentityService
    {
        Task<TokenResponse> GetAccessToken();
    }

    public class IdentityService : IIdentityService
    {
        private readonly HttpClient httpClient;
        private readonly IdentityServerConfigs identityServerConfigs;

        public IdentityService(HttpClient httpClient, IdentityServerConfigs identityServerConfigs)
        {
            this.httpClient = httpClient;
            this.identityServerConfigs = identityServerConfigs;
        }

        public async Task<TokenResponse> GetAccessToken()
        {
            var request = new DiscoveryDocumentRequest
            {
                Address = identityServerConfigs.Address,
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
                ClientId = identityServerConfigs.ClientId,
                ClientSecret = identityServerConfigs.ClientSecret,
                Scope = identityServerConfigs.Scope,
            });
        }
    }
}