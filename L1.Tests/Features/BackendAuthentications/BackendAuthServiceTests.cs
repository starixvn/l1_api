using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using IdentityModel.Client;
using L1.Features.BackendAuthentications;
using L1.Tests._Shared;
using NSubstitute;
using Xunit;


namespace L1.Tests.Features.BackendAuthentications
{
    public class BackendAuthServiceTests
    {
        private HttpClient httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;
        private readonly IDiscoveryCache discoveryCache;
        private readonly Fixture fixture = new Fixture();
        private BackendAuthService backendAuthService;


        public BackendAuthServiceTests()
        {
            httpClient = MockHttpClient.Create(HttpStatusCode.BadRequest, fixture.Create<string>());
            backendAuthConfigs = fixture.Create<BackendAuthConfigs>();
            discoveryCache = Substitute.For<IDiscoveryCache>();
        }


        [Fact]
        public void GetAccessToken_DiscoErrorIsFalse_InvokeRequestClientCredentialsTokenAsync()
        {
            // Arrange
            var expectedTokenReponse = fixture.Create<TokenResponse>();

            static bool isResponseTokenExpired(TokenResponse tokenResponse, long lastRequestTicks) => true;

            var disco = new DiscoveryDocumentResponse();
            discoveryCache.GetAsync().Returns(disco);


            var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = backendAuthConfigs.ClientId,
                ClientSecret = backendAuthConfigs.ClientSecret,
                Scope = backendAuthConfigs.Scope
            };

            var requestClientCredentialsTokenAsync
                = Substitute.For<Func<ClientCredentialsTokenRequest, CancellationToken, Task<TokenResponse>>>();

            requestClientCredentialsTokenAsync
                .Invoke(clientCredentialsTokenRequest, default)
                .Returns(expectedTokenReponse);

            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache, requestClientCredentialsTokenAsync);


            // Act
            var actual = backendAuthService.GetAccessToken(isResponseTokenExpired);

            // Assert
            requestClientCredentialsTokenAsync.ReceivedWithAnyArgs().Invoke(clientCredentialsTokenRequest, default);
        }
    }
}