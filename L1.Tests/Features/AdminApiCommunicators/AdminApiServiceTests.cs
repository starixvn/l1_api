using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.DailySummary;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.AdminApiCommunicators.RetrieveEndGameInfo;
using L1.Features.AdminApiCommunicators.UnderMaintenance;
using L1.Shared.Constants;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Sentry;
using Sentry.Protocol;
using Xunit;

namespace L1.Tests.Features.AdminApiCommunicators
{
    public class AdminApiServiceTests
    {
        private readonly AdminApiEndPoints adminApiEndPoints;
        private readonly ISentryClient sentryClient;

        public AdminApiServiceTests()
        {
            adminApiEndPoints = new AdminApiEndPoints
            {
                DailySummary = "/DailySummary",
                NotifyHighStake = "/NotifyHighStake",
                RetrieveEndGameInfo = "/RetrieveEndGameInfo",
                UnderMaintenance = "/UnderMaintenance"
            };

            sentryClient = Substitute.For<ISentryClient>();
        }

        [Theory, AutoData]
        public async Task DailySummary_Success_ReturnsDailySummaryResult(DailySummaryRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(string.Empty));
            var service = new AdminApiService(httpClient, adminApiEndPoints, sentryClient);

            // Act
            var result = await service.DailySummary(request);

            // Assert
            Assert.IsType<AdminApiResponse<DailySummaryResult>>(result);
        }

        [Theory, AutoData]
        public async Task DailySummary_Error_ReturnsDailySummaryResult(DailySummaryRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, JsonConvert.SerializeObject(string.Empty));
            var service = new AdminApiService(httpClient, adminApiEndPoints, sentryClient);

            // Act
            var response = await service.DailySummary(request);

            // Assert
            Assert.IsType<AdminApiResponse<DailySummaryResult>>(response);
            Assert.Equal(response.Result.ErrorCode, ErrorCodes.InternalError);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }

        [Theory, AutoData]
        public async Task RetrieveEndGameInfoAsync_Success_ReturnsRetrieveEndGameInfoResult(RetrieveEndGameInfoRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(string.Empty));
            var service = new AdminApiService(httpClient, adminApiEndPoints, sentryClient);

            // Act
            var result = await service.RetrieveEndGameInfoAsync(request);

            // Assert
            Assert.IsType<AdminApiResponse<RetrieveEndGameInfoResult>>(result);
        }

        [Theory, AutoData]
        public async Task UnderMaintenance_Success_Returns(MaintenanceRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(string.Empty));
            var service = new AdminApiService(httpClient, adminApiEndPoints, sentryClient);

            // Act
            await service.UnderMaintenance(request);
        }

        [Theory, AutoData]
        public async Task NotifyHighStake_Success_Returns(NotifyHighStakeRequest request)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(string.Empty));
            var service = new AdminApiService(httpClient, adminApiEndPoints, sentryClient);

            // Act
            await service.NotifyHighStake(request);
        }

        [Theory, AutoData]
        public void PostAndForget_Error_ShouldNotThrowError(string apiEndpoint)
        {
            // Arrange
            var httpClient = MockHttpClient.CreateErrorClient();
            var service = new AdminApiService(httpClient, adminApiEndPoints, sentryClient);

            // Act
            var actualResult = Assert.ThrowsAsync<Exception>(async () => await service.PostAndForget(apiEndpoint, null));

            // Assert
            Assert.NotNull(actualResult.Exception);
        }
    }
}