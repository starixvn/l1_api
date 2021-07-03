using System;
using System.Net.Http;
using AutoFixture;
using IdentityModel.Client;
using L1.Features.BackendAuthentications;
using NSubstitute;
using Xunit;


namespace L1.Tests.Features.BackendAuthentications
{
    public class BackendAuthServiceTests
    {
        private readonly HttpClient httpClient;
        private readonly BackendAuthConfigs backendAuthConfigs;
        private readonly IDiscoveryCache discoveryCache;
        private readonly Fixture fixture = new Fixture();
        private readonly BackendAuthService backendAuthService;


        public BackendAuthServiceTests()
        {
            //this.httpClient = NSubstitute.su;
            this.backendAuthConfigs = fixture.Create<BackendAuthConfigs>();
            this.discoveryCache = Substitute.For<IDiscoveryCache>();
            backendAuthService = new BackendAuthService(httpClient, backendAuthConfigs, discoveryCache);
        }


        [Fact]
        public void GetAccessToken_DiscoveryDocumentResponseErrorIsTrue_ThrowInvalidOperationException()
        {
            // Arrange
            bool isResponseTokenExpired(TokenResponse tokenResponse, long lastRequestTicks) => true;
            DiscoveryDocumentResponse disco = Substitute.For<DiscoveryDocumentResponse>();    
            discoveryCache.GetAsync().Returns(disco);

            //Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => backendAuthService.GetAccessToken(isResponseTokenExpired));
        }
    }
}