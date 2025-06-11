using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.HotdogCommunicators;
using L1.Tests._Shared;
using NSubstitute;
using Xunit;
using L1.Shared.Json;
using Sentry;

namespace L1.Tests.Features.HotdogCommunicators
{
    public class HotdogServiceTests
    {
        private readonly IHotdogFailureHandler failureHandler;
        private readonly IJsonService jsonService;
        private readonly object defaultContent = new
        {
            Result = new PartnerResult()
        };
        private readonly ISentryClient sentryClient;

        public HotdogServiceTests()
        {
            failureHandler = Substitute.For<IHotdogFailureHandler>();
            jsonService = Substitute.For<IJsonService>();
            sentryClient = Substitute.For<ISentryClient>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(HotdogCall call, HotdogServiceSettings oWServiceSettings)
        {
            // Arrange
            var serializedContent = jsonService.Serialize(defaultContent);
            jsonService.Serialize(Arg.Any<HotdogCall>()).Returns(serializedContent);
            jsonService.Deserialize<PartnerResponse<PartnerResult>>(serializedContent)
                .Returns(new PartnerResponse<PartnerResult> { Result = new PartnerResult() });

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedContent);
            var service = new HotdogService(httpClient, failureHandler, oWServiceSettings, jsonService, sentryClient);

            // Act
            var result = await service.Post<PartnerResult>("/aaa", call, "Hotdog");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(HotdogCall call, HotdogServiceSettings oWServiceSettings)
        {
            // Arrange
            var serializedContent = jsonService.Serialize(string.Empty);
            jsonService.Serialize(Arg.Any<HotdogCall>()).Returns(serializedContent);

            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, serializedContent);
            var service = new HotdogService(httpClient, failureHandler, oWServiceSettings, jsonService, sentryClient);

            // Act
            var response = await service.Post<PartnerResult>("/aaa", call, "Hotdog");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(PartnerSpecialErrorCodes.NetworkError, response.Result.ErrorCode);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}