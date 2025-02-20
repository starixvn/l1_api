using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using IdentityModel.Client;
using L1.Features.BackendAuthentications;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace L1.Tests.Features.BackendAuthentications
{
    public class BackendAuthServiceTests
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;
        private readonly IDiscoveryCache discoveryCache;
        private readonly Fixture fixture = new();
        private readonly ILogger<BackendAuthService> logger;
        private BackendAuthService backendAuthService;

        public BackendAuthServiceTests()
        {
            httpClient = Substitute.For<IHttpClientWrapper>();
            backendAuthConfigs = fixture.Create<BackendAuthConfigs>();
            discoveryCache = Substitute.For<IDiscoveryCache>();
            logger = Substitute.For<ILogger<BackendAuthService>>();
        }

        [Fact]
        public void GetAccessToken_DefaultValidationMethod_ReturnExpectedResponeToken()
        {
            // Arrange       
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
            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache, logger);

            // Act
            var actual = backendAuthService.GetAccessToken().Result;

            // Assert
            Assert.Equal(expectedTokenReponse, actual);
        }

        [Fact]
        public async Task GetAccessToken_DiscoveryCacheError_ShouldThrowInvalidOperationException()
        {
            // Arrange       
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
            var disco = await ProtocolResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(httpResponse);
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

            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache, logger);

            // Act
            var result = Assert.ThrowsAsync<InvalidOperationException>(async () => await backendAuthService.GetAccessToken());

            // Assert
            Assert.Null(result.Exception);
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

            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache, logger);

            // Act
            var actual = backendAuthService.GetAccessToken(isResponseTokenExpired).Result;

            // Assert
            Assert.Equal(expectedTokenReponse, actual);
        }

        [Fact]
        public void GetAccessToken_Error_ShouldThrowException()
        {
            // Arrange
            static bool isResponseTokenExpired(TokenResponse tokenResponse, long lastRequestTicks) => true;
            var disco = new DiscoveryDocumentResponse();
            discoveryCache.GetAsync().Throws(new Exception());

            var clientCredentials = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = backendAuthConfigs.ClientId,
                ClientSecret = backendAuthConfigs.ClientSecret,
                Scope = backendAuthConfigs.Scope
            };

            var expectedTokenReponse = new TokenResponse();
            httpClient.RequestClientCredentialsTokenAsync(clientCredentials, default).ReturnsForAnyArgs(expectedTokenReponse);

            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache, logger);

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await backendAuthService.GetAccessToken(isResponseTokenExpired));
        }

        [Fact]
        public async Task GetAccessToken_TokenExpired_ReturnExpectedResponeToken()
        {
            // Arrange
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

            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache, logger);
            await backendAuthService.GetAccessToken();

            // Act
            var actual = backendAuthService.GetAccessToken().Result;

            // Assert
            Assert.Equal(expectedTokenReponse, actual);
        }

        [Fact]
        public async Task GetAccessToken_TokenIsNotExpired_ReturnExpectedResponeToken()
        {
            // Arrange
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

            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache, logger);
            await backendAuthService.GetAccessToken();

            // Act
            var actual = backendAuthService.GetAccessToken().Result;

            // Assert
            Assert.Equal(expectedTokenReponse, actual);
        }
    }
}