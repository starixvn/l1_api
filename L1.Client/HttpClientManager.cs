using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace L1.Client
{
    public static class HttpClientManager
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetAccessToken(AuthenticationInfo authenticationInfo)
        {
            var disco = await client.GetDiscoveryDocumentAsync(authenticationInfo.AuthUri.ToString());
            if (disco.IsError)
            {
                throw new InvalidOperationException(disco.Error);
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = authenticationInfo.ClientId,
                ClientSecret = authenticationInfo.ClientSecret,
                Scope = authenticationInfo.SiteName
            });

            if (tokenResponse.IsError)
            {
                throw new InvalidOperationException(tokenResponse.Error);
            }

            return tokenResponse.AccessToken;
        }
    }
}