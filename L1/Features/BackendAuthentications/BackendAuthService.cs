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
        Task<TokenResponse> GetAccessToken(Func<TokenResponse, long, bool> isResponseTokenExpired = null);
    }

    public class BackendAuthService : IBackendAuthService
    {
        private static TokenResponse TokenResponse;
        private static long LastRequestTicks;

        private readonly HttpClient httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;
        private readonly IDiscoveryCache discoveryCache;

        public BackendAuthService(HttpClient httpClient, BackendAuthConfigs backendAuthConfigs, IDiscoveryCache discoveryCache)
        {
            this.httpClient = httpClient;
            this.backendAuthConfigs = backendAuthConfigs;
            this.discoveryCache = discoveryCache;
        }

        public async Task<TokenResponse> GetAccessToken()
            =>  await GetAccessToken(null);

        public async Task<TokenResponse> GetAccessToken(Func<TokenResponse, long, bool> isResponseTokenExpired)
        {
            isResponseTokenExpired ??= IsResponseTokenExpired; 
          
            if (isResponseTokenExpired(TokenResponse, LastRequestTicks))
            {
                var disco = await discoveryCache.GetAsync();

                if (!disco.IsError)
                {

                    var newTokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
                        new ClientCredentialsTokenRequest
                        {
                            Address = disco.TokenEndpoint,
                            ClientId = backendAuthConfigs.ClientId,
                            ClientSecret = backendAuthConfigs.ClientSecret,
                            Scope = backendAuthConfigs.Scope,
                        });

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

        private  bool IsResponseTokenExpired(TokenResponse tokenResponse, long lastRequestTicks)
                => tokenResponse == null || new DateTimeOffset(lastRequestTicks, TimeSpan.Zero)
                        .AddSeconds(tokenResponse.ExpiresIn) < DateTimeOffset.UtcNow.AddMinutes(1);
    }
}