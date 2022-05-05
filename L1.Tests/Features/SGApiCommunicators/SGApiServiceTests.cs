using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using L1.Features.Partners;
using L1.Features.SGApiCommunicators;
using L1.Shared.Constants;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.SGApiCommunicators
{
    public class SGApiServiceTests
    {
        private readonly SGApiEndPoints sgApiEndPoints;
        private readonly ISentryClient sentryClient;
        private readonly IPartnerDataService partnerDataService;
        private readonly Fixture fixture = new();

        public SGApiServiceTests()
        {
            sgApiEndPoints = new SGApiEndPoints
            {
                DailySummary = "/DailySummary",
                RetrieveEndRoundInfo = "/RetrieveEndRoundInfo",
            };

            sentryClient = Substitute.For<ISentryClient>();
            partnerDataService = Substitute.For<IPartnerDataService>();
            partnerDataService.GetById(Arg.Any<string>()).Returns(fixture.Create<Partner>());
        }

        [Theory, AutoData]
        public async Task DailySummary_Success_ReturnsDailySummaryResult(DailySummaryRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(fixture.Create<SGResponse<DailySummaryResult>>()));
            var service = new SGApiService(httpClient, sgApiEndPoints, sentryClient, partnerDataService);

            // Act
            var result = await service.DailySummary(request);

            // Assert
            Assert.IsType<SGResponse<DailySummaryResult>>(result);
        }

        [Theory, AutoData]
        public async Task DailySummary_Error_ReturnsDailySummaryResult(DailySummaryRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, JsonConvert.SerializeObject(string.Empty));
            var service = new SGApiService(httpClient, sgApiEndPoints, sentryClient, partnerDataService);

            // Act
            var response = await service.DailySummary(request);

            // Assert
            Assert.IsType<SGResponse<DailySummaryResult>>(response);
            Assert.Equal(response.Result.ErrorCode, ErrorCodes.InternalError);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }

        [Theory, AutoData]
        public async Task RetrieveEndGameInfoAsync_Success_ReturnsRetrieveEndGameInfoResult(RetrieveEndRoundInfoRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(fixture.Create<SGResponse<RetrieveEndRoundInfoResult>>()));
            var service = new SGApiService(httpClient, sgApiEndPoints, sentryClient, partnerDataService);

            // Act
            var result = await service.RetrieveEndRoundInfoAsync(request);

            // Assert
            Assert.IsType<SGResponse<RetrieveEndRoundInfoResult>>(result);
        }
    }
}