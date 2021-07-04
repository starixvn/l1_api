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

        Task<TokenResponse> GetAccessToken(Func<TokenResponse, long, bool> isResponseTokenExpired);
    }

    public class BackendAuthService : IBackendAuthService
    {
        private static TokenResponse TokenResponse;
        private static long LastRequestTicks;

        private readonly HttpClient httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;
        private readonly IDiscoveryCache discoveryCache;
        private readonly Func<ClientCredentialsTokenRequest, CancellationToken, Task<TokenResponse>> requestClientCredentialsTokenAsync;

        public BackendAuthService(HttpClient httpClient,
            BackendAuthConfigs backendAuthConfigs,
            IDiscoveryCache discoveryCache,
            Func<ClientCredentialsTokenRequest, CancellationToken, Task<TokenResponse>> requestClientCredentialsTokenAsync = null)
        {
            this.httpClient = httpClient;
            this.backendAuthConfigs = backendAuthConfigs;
            this.discoveryCache = discoveryCache;
            this.requestClientCredentialsTokenAsync = requestClientCredentialsTokenAsync;
        }

        public async Task<TokenResponse> GetAccessToken()
            => await GetAccessToken(null);

        public async Task<TokenResponse> GetAccessToken(
            Func<TokenResponse, long, bool> isResponseTokenExpired)
        {
            isResponseTokenExpired ??= IsResponseTokenExpired;

            if (isResponseTokenExpired(TokenResponse, LastRequestTicks))
            {
                var disco = await discoveryCache.GetAsync();

                if (!disco.IsError)
                {
                    var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = backendAuthConfigs.ClientId,
                        ClientSecret = backendAuthConfigs.ClientSecret,
                        Scope = backendAuthConfigs.Scope,
                    };

                    var newTokenResponse = requestClientCredentialsTokenAsync == null
                        ? await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest)
                        : await requestClientCredentialsTokenAsync.Invoke(clientCredentialsTokenRequest, default);

                    Interlocked.Exchange(ref TokenResponse, newTokenResponse);
                    Interlocked.Exchange(ref LastRequestTicks, DateTimeOffset.UtcNow.Ticks);
                }
                else
                {
                    throw new InvalidOperationException(disco.Error);
                }
            }

            return TokenResponse;
        }

        private static bool IsResponseTokenExpired(TokenResponse tokenResponse, long lastRequestTicks)
                => tokenResponse == null || new DateTimeOffset(lastRequestTicks, TimeSpan.Zero)
                        .AddSeconds(tokenResponse.ExpiresIn) < DateTimeOffset.UtcNow.AddMinutes(1);
    }
}