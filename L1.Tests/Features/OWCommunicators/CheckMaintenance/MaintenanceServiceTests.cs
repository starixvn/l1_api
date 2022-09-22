using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using L1.Features.AdminApiCommunicators;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Shared.Configurations;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.OWCommunicators.CheckMaintenance
{
    public class MaintenanceServiceTests
    {
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly OWServiceSettings owServiceSettings;
        private readonly Fixture fixture = new Fixture();
        private readonly IAppSettings appSettings;

        public MaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();
            appSettings = Substitute.For<IAppSettings>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO"
            };

            owServiceSettings = new OWServiceSettings
            {
                BaseUrl = new Uri("http://localhost/"),
                Endpoints = new OWEndPoints
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

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new OWResponse<MaintenanceResult> { Result = maintenanceResult }));

            // Act
            var maintenanceService = new OWMaintenanceService(httpClient, owServiceSettings, adminApiService, sentryClient, appSettings);

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

            var httpClient = MockHttpClient.Create(HttpStatusCode.NotFound, JsonConvert.SerializeObject(new OWResponse<MaintenanceResult> { Result = maintenanceResult }));
            var maintenanceService = new OWMaintenanceService(httpClient, owServiceSettings, adminApiService, sentryClient, appSettings);
            var call = fixture.Create<OWCall>();

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(call));

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

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new OWResponse<MaintenanceResult> { Result = maintenanceResult }));
            var maintenanceService = new OWMaintenanceService(httpClient, owServiceSettings, adminApiService, sentryClient, appSettings);
            var call = fixture.Create<OWCall>();

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(call));

            // Assert
            Assert.True(maintenanceService.IsUnderMaintenance());
        }
    }
}