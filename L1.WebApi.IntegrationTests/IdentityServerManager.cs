using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace L1.WebApi.IntegrationTests
{
    public static class IdentityServerManager
    {
        private static HttpClient identityClient;
        private static readonly object lockObj = new();
        private static IdentityServerAuth auth;
        private static TokenResponse tokenResponse;
        private static DateTime lastTokenRequest;
        private static IDiscoveryCache discoveryCache;

        public static void Initialize(IdentityServerAuth identityServerAuth)
        {
            auth = identityServerAuth;
            discoveryCache = new DiscoveryCache(identityServerAuth.Url, new DiscoveryPolicy
            {
                RequireHttps = false
            });
        }

        public static async Task<string> GetAccessToken()
        {
            if (tokenResponse == null || lastTokenRequest.AddSeconds(tokenResponse.ExpiresIn) < DateTime.Now.AddMinutes(5))
            {
                var client = GetIdentityClient();

                var disco = await discoveryCache.GetAsync();

                if (disco.IsError)
                {
                    throw new InvalidOperationException(disco.Error);
                }

                tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = auth.ClientId,
                    ClientSecret = auth.ClientSecret,
                    Scope = auth.SiteId,
                    Method = HttpMethod.Post
                });

                lastTokenRequest = DateTime.Now;
            }

            return tokenResponse.AccessToken;
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
    }

    public class IdentityServerAuth
    {
        public string Url { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string SiteId { get; set; }
    }
}