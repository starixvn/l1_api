using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Tests._Shared;
using NSubstitute;
using Xunit;
using L1.Shared.Json;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWServiceTests
    {
        private readonly IOWFailureHandler failureHandler;
        private readonly IJsonService jsonService;
        private readonly OWResponse<PartnerResult> defaultContent = new()
        {
            Result = new PartnerResult()
        };

        public OWServiceTests()
        {
            failureHandler = Substitute.For<IOWFailureHandler>();
            jsonService = Substitute.For<IJsonService>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(OWCall call, OWServiceSettings oWServiceSettings)
        {
            // Arrange
            var serializedContent = jsonService.Serialize(defaultContent);
            jsonService.Serialize(Arg.Any<PartnerRequest>()).Returns(serializedContent);
            jsonService.Deserialize<PartnerResponse<PartnerResult>>(serializedContent)
                .Returns(new PartnerResponse<PartnerResult> { Result = defaultContent.Result });

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedContent);
            var service = new OWService(httpClient, failureHandler, oWServiceSettings, jsonService);

            // Act
            var result = await service.Post<PartnerResult>("/aaa", call, "OW");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(OWCall call, OWServiceSettings oWServiceSettings)
        {
            // Arrange
            var serializedContent = jsonService.Serialize(string.Empty);
            jsonService.Serialize(Arg.Any<PartnerRequest>()).Returns(serializedContent);

            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, serializedContent);
            var service = new OWService(httpClient, failureHandler, oWServiceSettings, jsonService);

            // Act
            var response = await service.Post<PartnerResult>("/aaa", call, "OW");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(PartnerSpecialErrorCodes.NetworkError, response.Result.ErrorCode);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}