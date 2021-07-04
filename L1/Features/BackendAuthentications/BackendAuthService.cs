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

    public interface IHttpClientWrapper
    {
        Task<TokenResponse> RequestClientCredentialsTokenAsync(
            ClientCredentialsTokenRequest clientCredentials,
            CancellationToken cancellationToken);
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<TokenResponse> RequestClientCredentialsTokenAsync(ClientCredentialsTokenRequest clientCredentials, CancellationToken cancellationToken)
             => httpClient.RequestClientCredentialsTokenAsync(clientCredentials, cancellationToken);
    }

    public class BackendAuthService : IBackendAuthService
    {
        private static TokenResponse TokenResponse;
        private static long LastRequestTicks;

        private readonly IHttpClientWrapper httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;
        private readonly IDiscoveryCache discoveryCache;


        public BackendAuthService(IHttpClientWrapper httpClient,
            BackendAuthConfigs backendAuthConfigs,
            IDiscoveryCache discoveryCache)
        {
            this.httpClient = httpClient;
            this.backendAuthConfigs = backendAuthConfigs;
            this.discoveryCache = discoveryCache;
        }

        public async Task<TokenResponse> GetAccessToken()
            => await GetAccessToken(null);

        public async Task<TokenResponse> GetAccessToken(Func<TokenResponse, long, bool> checkResponseTokenFunc)
        {
            checkResponseTokenFunc ??= IsResponseTokenExpired;

            if (checkResponseTokenFunc(TokenResponse, LastRequestTicks))
            {
                var disco = await discoveryCache.GetAsync();

                if (!disco.IsError)
                {
                    var clientCredentials = new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = backendAuthConfigs.ClientId,
                        ClientSecret = backendAuthConfigs.ClientSecret,
                        Scope = backendAuthConfigs.Scope,
                    };

                    var newTokenResponse = await httpClient.RequestClientCredentialsTokenAsync(clientCredentials, default);

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