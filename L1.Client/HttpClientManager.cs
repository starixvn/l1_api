using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace L1.Client
{
    public static class HttpClientManager
    {
        private static HttpClient identityClient;
        private static HttpClient apiClient;
        private static readonly object lockObj = new object();

        public static async Task<TokenResponse> GetAccessToken(AuthenticationInfo authenticationInfo)
        {
            var client = GetIdentityClient();

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

        private static HttpClient GetIdentityClient()
        {
            if (identityClient == null)
            {
                lock (lockObj)
                {
                    if (identityClient == null)
                    {
                        identityClient = new HttpClient();
                    }
                }
            }

            return identityClient;
        }

        public static HttpClient GetApiClient(Uri apiUri)
        {
            if (apiClient == null)
            {
                lock (lockObj)
                {
                    if (apiClient == null)
                    {
                        apiClient = new HttpClient { BaseAddress = apiUri };
                    }
                }
            }

            return apiClient;
        }
    }
}