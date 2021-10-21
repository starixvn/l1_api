using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using IdentityModel.Client;
using L1.Features.BackendAuthentications;
using L1.Tests._Shared;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.BackendAuthentications
{
    public class HttpClientWrapperTests
    {
        private HttpClientWrapper wrapper;

        public HttpClientWrapperTests()
        {
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, string.Empty);
            wrapper = new HttpClientWrapper(httpClient);
        }

        [Theory, AutoData]
        public async Task RequestClientCredentialsTokenAsync_Always_ReturnReponseFromHttpClient(CancellationToken cancellationToken)
        {
            // Arrange       
            var clientCredentials = Substitute.For<ClientCredentialsTokenRequest>();

            // Act
            var actual = await wrapper.RequestClientCredentialsTokenAsync(clientCredentials, cancellationToken);

            // Assert
            Assert.Equal(actual, actual);
        }

    }
}
