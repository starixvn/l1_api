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
        private readonly BackendAuthService backendAuthService;


        public BackendAuthServiceTests()
        {
            httpClient = MockHttpClient.Create(HttpStatusCode.BadRequest, fixture.Create<string>());
            backendAuthConfigs = fixture.Create<BackendAuthConfigs>();
            discoveryCache = Substitute.For<IDiscoveryCache>();
            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache);
        }


        [Fact]
        public void GetAccessToken_DiscoErrorIsTrue_HttpClientInvokeRequestClientCredentialsTokenAsync()
        {
            // Arrange
            //TokenResponse expectedTokenReponse = fixture.Create<TokenResponse>();
            //bool isResponseTokenExpired(TokenResponse tokenResponse, long lastRequestTicks) => true;
            //DiscoveryDocumentResponse disco = Substitute.For<DiscoveryDocumentResponse>();    
            //discoveryCache.GetAsync().Returns(disco);
            //var clientCredentialsToken = new ClientCredentialsTokenRequest
            //{
            //    Address = disco.TokenEndpoint,
            //    ClientId = backendAuthConfigs.ClientId,
            //    ClientSecret = backendAuthConfigs.ClientSecret,
            //    Scope = backendAuthConfigs.Scope,
            //};
      


            //// Act
            //var actual =  backendAuthService.GetAccessToken(isResponseTokenExpired);

            //// Assert
            //httpClient.Received().RequestClientCredentialsTokenAsync(clientCredentialsToken);
        }
    }
}