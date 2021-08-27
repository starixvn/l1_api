using AutoFixture;
using IdentityModel.Client;
using L1.Features.BackendAuthentications;
using NSubstitute;
using Xunit;


namespace L1.Tests.Features.BackendAuthentications
{
    public class BackendAuthServiceTests
    {
        private IHttpClientWrapper httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;
        private readonly IDiscoveryCache discoveryCache;
        private readonly Fixture fixture = new Fixture();
        private BackendAuthService backendAuthService;


        public BackendAuthServiceTests()
        {
            httpClient = Substitute.For<IHttpClientWrapper>();
            backendAuthConfigs = fixture.Create<BackendAuthConfigs>();
            discoveryCache = Substitute.For<IDiscoveryCache>();
        }


        [Fact]
        public void GetAccessToken_ResponseIsOk_ReturnExpectedResponeToken()
        {
            // Arrange       
            static bool isResponseTokenExpired(TokenResponse tokenResponse, long lastRequestTicks) => true;
            var disco = new DiscoveryDocumentResponse();
            discoveryCache.GetAsync().Returns(disco);

            var clientCredentials = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = backendAuthConfigs.ClientId,
                ClientSecret = backendAuthConfigs.ClientSecret,
                Scope = backendAuthConfigs.Scope
            };

            var expectedTokenReponse = new TokenResponse();
            httpClient.RequestClientCredentialsTokenAsync(clientCredentials, default).ReturnsForAnyArgs(expectedTokenReponse);

            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache);


            // Act
            var actual = backendAuthService.GetAccessToken(isResponseTokenExpired).Result;

            // Assert
            Assert.Equal(expectedTokenReponse, actual);
;        }        
    }
}