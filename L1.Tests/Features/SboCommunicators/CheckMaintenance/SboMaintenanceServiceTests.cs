using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using L1.Features.AdminApiCommunicators;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.SboCommunicators;
using L1.Features.SboCommunicators.CheckMaintenance;
using L1.Shared.Configurations;
using L1.Tests._Shared;
using NSubstitute;
using Sentry;
using Xunit;
using L1.Features.Json;
using System.Text.Json;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;

namespace L1.Tests.Features.SboCommunicators.CheckMaintenance
{
    public class SboMaintenanceServiceTests
    {
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;
        private readonly SboServiceSettings sboServiceSettings;
        private readonly Fixture fixture = new();
        private readonly IAppSettings appSettings;

        public SboMaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();
            jsonService = Substitute.For<IJsonService>();
            appSettings = Substitute.For<IAppSettings>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO"
            };

            sboServiceSettings = new SboServiceSettings
            {
                BaseUrl = new Uri("http://localhost/"),
                Endpoints = new SboEndPoints
                {
                    CheckMaintenance = "maintenance"
                }
            };
        }

        [Fact]
        public void IsUnderMaintenance_CounterIsZero_ReturnTrue()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(5)
            };
            var response = new SboResponse<MaintenanceResult> { Result = maintenanceResult };
            var serializedContent = JsonSerializer.Serialize(response);
            jsonService.Deserialize<SboResponse<MaintenanceResult>>(serializedContent).Returns(response);

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedContent);
            var maintenanceService = new SboMaintenanceService(httpClient, sboServiceSettings, adminApiService, sentryClient, appSettings, jsonService);

            // Assert
            Assert.True(maintenanceService.IsUnderMaintenance());
        }

        [Fact]
        public async Task IsUnderMaintenance_CounterIsNotZero_ReturnFalse()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(5)
            };
            var response = new SboResponse<MaintenanceResult> { Result = maintenanceResult };
            var serializedContent = JsonSerializer.Serialize(response);
            jsonService.Deserialize<SboResponse<MaintenanceResult>>(serializedContent).Returns(response);

            var httpClient = MockHttpClient.Create(HttpStatusCode.NotFound, serializedContent);
            var maintenanceService = new SboMaintenanceService(httpClient, sboServiceSettings, adminApiService, sentryClient, appSettings, jsonService);
            var call = fixture.Create<SboCall>();

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new SboRequest(call));

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
            var maintenanceService = new SboMaintenanceService(httpClient, sboServiceSettings, adminApiService, sentryClient, appSettings, jsonService);
            var call = fixture.Create<SboCall>();

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new SboRequest(call));

            // Assert
            Assert.True(maintenanceService.IsUnderMaintenance());
        }
    }
}