using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWServiceTests
    {
        private readonly IOWFailureHandler failureHandler;

        private readonly OWResponse<PartnerResult> defaultContent = new()
        {
            Result = new PartnerResult()
        };

        public OWServiceTests()
        {
            failureHandler = Substitute.For<IOWFailureHandler>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(OWCall call, OWServiceSettings oWServiceSettings)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(defaultContent));
            var service = new OWService(httpClient, failureHandler, oWServiceSettings);

            // Act
            var result = await service.Post<PartnerResult>("/aaa", call, "OW");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(OWCall call, OWServiceSettings oWServiceSettings)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, JsonConvert.SerializeObject(string.Empty));
            var service = new OWService(httpClient, failureHandler, oWServiceSettings);

            // Act
            var response = await service.Post<PartnerResult>("/aaa", call, "OW");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(PartnerSpecialErrorCodes.NetworkError, response.Result.ErrorCode);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}