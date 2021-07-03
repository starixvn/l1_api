using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.OWCommunicators;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWServiceTests
    {
        private readonly IOWAuth auth;
        private readonly IOWFailureHandler failureHandler;

        private readonly OWResponse<OWResult> defaultContent = new OWResponse<OWResult>
        {
            Result = new OWResult()
        };

        public OWServiceTests()
        {
            auth = Substitute.For<IOWAuth>();
            failureHandler = Substitute.For<IOWFailureHandler>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(OWCall call)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(defaultContent));
            var service = new OWService(httpClient, auth, failureHandler);

            // Act
            var result = await service.Post<OWResult>("https://localhost", call);

            // Assert
            Assert.IsType<OWResponse<OWResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(OWCall call)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, JsonConvert.SerializeObject(string.Empty));
            var service = new OWService(httpClient, auth, failureHandler);

            // Act
            var response = await service.Post<OWResult>("https://localhost", call);

            // Assert
            Assert.IsType<OWResponse<OWResult>>(response);
            Assert.Equal(response.Result.ErrorCode, OWSpecialErrorCodes.NetworkError);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}