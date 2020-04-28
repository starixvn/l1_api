using System;
using System.Net.Http;
using System.Threading;
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
        private static TokenResponse TokenResponse;
        private static DateTimeOffset LastTokenRequestTime;

        private readonly HttpClient httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;

        public BackendAuthService(HttpClient httpClient, BackendAuthConfigs backendAuthConfigs)
        {
            this.httpClient = httpClient;
            this.backendAuthConfigs = backendAuthConfigs;
        }

        public async Task<TokenResponse> GetAccessToken()
        {
            if (TokenResponse == null || LastTokenRequestTime.AddSeconds(TokenResponse.ExpiresIn) < DateTimeOffset.UtcNow.AddMinutes(1))
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

                var newTokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = backendAuthConfigs.ClientId,
                    ClientSecret = backendAuthConfigs.ClientSecret,
                    Scope = backendAuthConfigs.Scope,
                });

                Interlocked.Exchange(ref TokenResponse, newTokenResponse);
                LastTokenRequestTime = DateTimeOffset.UtcNow;
            }

            return TokenResponse;
        }
    }
}