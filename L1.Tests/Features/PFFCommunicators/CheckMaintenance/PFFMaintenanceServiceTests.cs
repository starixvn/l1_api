using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using L1.Features.AdminApiCommunicators;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.PFFCommunicators;
using L1.Features.PFFCommunicators.CheckMaintenance;
using L1.Shared.Configurations;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.PFFCommunicators.CheckMaintenance
{
    public class PFFMaintenanceServiceTests
    {
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly PFFServiceSettings PFFServiceSettings;
        private readonly Fixture fixture = new();
        private readonly IAppSettings appSettings;

        public PFFMaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();
            appSettings = Substitute.For<IAppSettings>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO",
                PFF = "PFF"
            };

            PFFServiceSettings = new PFFServiceSettings
            {
                BaseUrl = new Uri("http://localhost/"),
                Endpoints = new PFFEndPoints
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

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new { Result = maintenanceResult }));

            // Act
            var maintenanceService = new PFFMaintenanceService(httpClient, PFFServiceSettings, adminApiService, sentryClient, appSettings);

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

            var httpClient = MockHttpClient.Create(HttpStatusCode.NotFound, JsonConvert.SerializeObject(new { Result = maintenanceResult }));
            var maintenanceService = new PFFMaintenanceService(httpClient, PFFServiceSettings, adminApiService, sentryClient, appSettings);
            var call = fixture.Create<PFFCall>();

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new PFFRequest(call));

            // Assert
            Assert.False(maintenanceService.IsUnderMaintenance());
        }

        [Fact]
        public async Task ObserveMaintenanceSchedule_Success_ShouldResetCounterToZero()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(500)
            };

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new { Result = maintenanceResult }));
            var maintenanceService = new PFFMaintenanceService(httpClient, PFFServiceSettings, adminApiService, sentryClient, appSettings);
            var call = fixture.Create<PFFCall>();
            await maintenanceService.ObserveMaintenanceSchedule(new PFFRequest(call));

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new PFFRequest(call));

            // Assert
            Assert.True(maintenanceService.IsUnderMaintenance());
        }
    }
}