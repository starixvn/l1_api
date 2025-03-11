using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.PFFCommunicators;
using L1.Tests._Shared;
using NSubstitute;
using Xunit;
using L1.Shared.Services;

namespace L1.Tests.Features.PFFCommunicators
{
    public class PFFServiceTests
    {
        private readonly IPFFFailureHandler failureHandler;
        private readonly IJsonService jsonService;
        private readonly object defaultContent = new
        {
            Result = new PartnerResult()
        };

        public PFFServiceTests()
        {
            failureHandler = Substitute.For<IPFFFailureHandler>();
            jsonService = Substitute.For<IJsonService>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(PFFCall call, PFFServiceSettings oWServiceSettings)
        {
            // Arrange
            var serializedContent = jsonService.Serialize(defaultContent);
            jsonService.Serialize(Arg.Any<PFFCall>()).Returns(serializedContent);
            jsonService.Deserialize<PartnerResponse<PartnerResult>>(serializedContent)
                .Returns(new PartnerResponse<PartnerResult> { Result = new PartnerResult() });

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedContent);
            var service = new PFFService(httpClient, failureHandler, oWServiceSettings, jsonService);

            // Act
            var result = await service.Post<PartnerResult>("/aaa", call);

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(PFFCall call, PFFServiceSettings oWServiceSettings)
        {
            // Arrange
            var serializedContent = jsonService.Serialize(string.Empty);
            jsonService.Serialize(Arg.Any<PFFCall>()).Returns(serializedContent);

            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, serializedContent);
            var service = new PFFService(httpClient, failureHandler, oWServiceSettings, jsonService);

            // Act
            var response = await service.Post<PartnerResult>("/aaa", call);

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(PartnerSpecialErrorCodes.NetworkError, response.Result.ErrorCode);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}