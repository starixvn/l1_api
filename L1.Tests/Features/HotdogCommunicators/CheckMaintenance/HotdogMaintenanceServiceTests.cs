using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using L1.Features.AdminApiCommunicators;
using L1.Features.HotdogCommunicators;
using L1.Features.HotdogCommunicators.CheckMaintenance;
using L1.Shared.Configurations;
using L1.Tests._Shared;
using NSubstitute;
using Sentry;
using Xunit;
using System.Text.Json;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Json;

namespace L1.Tests.Features.HotdogCommunicators.CheckMaintenance
{
    public class HotdogMaintenanceServiceTests
    {
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;
        private readonly HotdogServiceSettings HotdogServiceSettings;
        private readonly Fixture fixture = new();
        private readonly IAppSettings appSettings;

        public HotdogMaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();
            jsonService = Substitute.For<IJsonService>();
            appSettings = Substitute.For<IAppSettings>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO",
                SabaPlay_HD = "Hotdog"
            };

            HotdogServiceSettings = new HotdogServiceSettings
            {
                BaseUrl = new Uri("http://localhost/"),
                Endpoints = new HotdogEndPoints
                {
                    CheckMaintenance = "maintenance"
                }
            };
        }

        [Fact]
        public void IsUnderMaintenance_CounterIsZero_ReturnTrue()
        {
            // Arrange
            var maintenanceResult = new PartnerMaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(5)
            };
            var response = new PartnerResponse<PartnerMaintenanceResult> { Result = maintenanceResult };
            var serializedContent = JsonSerializer.Serialize(response);
            jsonService.Deserialize<PartnerResponse<PartnerMaintenanceResult>>(serializedContent).Returns(response);

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedContent);
            var maintenanceService = new HotdogMaintenanceService(httpClient, HotdogServiceSettings, adminApiService, sentryClient, jsonService);

            // Assert
            Assert.True(maintenanceService.IsUnderMaintenance());
        }

        [Fact]
        public async Task IsUnderMaintenance_CounterIsNotZero_ReturnFalse()
        {
            // Arrange
            var maintenanceResult = new PartnerMaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(5)
            };
            var response = new PartnerResponse<PartnerMaintenanceResult> { Result = maintenanceResult };
            var serializedContent = JsonSerializer.Serialize(response);
            jsonService.Deserialize<PartnerResponse<PartnerMaintenanceResult>>(serializedContent).Returns(response);

            var httpClient = MockHttpClient.Create(HttpStatusCode.NotFound, serializedContent);
            var maintenanceService = new HotdogMaintenanceService(httpClient, HotdogServiceSettings, adminApiService, sentryClient, jsonService);
            var call = fixture.Create<HotdogCall>();

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new HotdogRequest(call), appSettings.PartnerIds.SabaPlay_HD);

            // Assert
            Assert.False(maintenanceService.IsUnderMaintenance());
        }

        [Fact]
        public async Task ObserveMaintenanceSchedule_Success_ShouldResetCounterToZero()
        {
            // Arrange
            var maintenanceResult = new PartnerMaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(500)
            };
            var response = new PartnerResponse<PartnerMaintenanceResult> { Result = maintenanceResult };
            var serializedContent = JsonSerializer.Serialize(response);
            jsonService.Deserialize<PartnerResponse<PartnerMaintenanceResult>>(serializedContent).Returns(response);

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedContent);
            var maintenanceService = new HotdogMaintenanceService(httpClient, HotdogServiceSettings, adminApiService, sentryClient, jsonService);
            var call = fixture.Create<HotdogCall>();

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new HotdogRequest(call), appSettings.PartnerIds.SabaPlay_HD);

            // Assert
            Assert.True(maintenanceService.IsUnderMaintenance());
        }
    }
}