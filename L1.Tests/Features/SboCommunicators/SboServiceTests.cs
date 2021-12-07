using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.SboCommunicators;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.SboCommunicators
{
    public class SboServiceTests
    {
        private readonly ISboFailureHandler failureHandler;

        private readonly SboResponse<PartnerResult> defaultContent = new()
        {
            Result = new PartnerResult()
        };

        public SboServiceTests()
        {
            failureHandler = Substitute.For<ISboFailureHandler>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(SboCall call, SboServiceSettings oWServiceSettings)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(defaultContent));
            var service = new SboService(httpClient, failureHandler, oWServiceSettings);

            // Act
            var result = await service.Post<PartnerResult>("/aaa", call);

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(SboCall call, SboServiceSettings oWServiceSettings)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, JsonConvert.SerializeObject(string.Empty));
            var service = new SboService(httpClient, failureHandler, oWServiceSettings);

            // Act
            var response = await service.Post<PartnerResult>("/aaa", call);

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(response.Result.ErrorCode, PartnerSpecialErrorCodes.NetworkError);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}