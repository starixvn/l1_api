using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace L1.Client
{
    public static class HttpClientManager
    {
        public static async Task<TokenResponse> GetAccessToken(HttpClient client, AuthenticationInfo authenticationInfo)
        {
            var request = new DiscoveryDocumentRequest
            {
                Address = authenticationInfo.AuthUri.ToString(),
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = false
                }
            };
            var disco = await client.GetDiscoveryDocumentAsync(request);
            if (disco.IsError)
            {
                throw new InvalidOperationException(disco.Error);
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = authenticationInfo.ClientId,
                ClientSecret = authenticationInfo.ClientSecret,
                Scope = authenticationInfo.SiteId,
            });

            return tokenResponse;
        }
    }
}