using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.OWCommunicators.CheckMaintenance
{
    public class MaintenanceServiceTests
    {
        private const string DefaultMediaType = "application/json";

        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly OWServiceSettings owServiceSettings;
        private readonly Fixture fixture = new Fixture();

        public MaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();

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
        public async Task IsUnderMaintenance_CounterIsZero_ReturnTrue()
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(5)
            };

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new OWResponse<MaintenanceResult> { Result = maintenanceResult }));
            var maintenanceService = new MaintenanceService(httpClient, owServiceSettings, adminApiService, sentryClient);

            // Act
            var actualValue = await maintenanceService.IsUnderMaintenance();

            // Assert
            Assert.True(actualValue);
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
            var maintenanceService = new MaintenanceService(httpClient, owServiceSettings, adminApiService, sentryClient);
            var call = fixture.Create<OWCall>();
            await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(null, call));

            // Act
            var actualValue = await maintenanceService.IsUnderMaintenance();

            // Assert
            Assert.False(actualValue);
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
            var maintenanceService = new MaintenanceService(httpClient, owServiceSettings, adminApiService, sentryClient);
            var call = fixture.Create<OWCall>();
            await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(null, call));

            // Act
            var actualValue = await maintenanceService.IsUnderMaintenance();

            // Assert
            Assert.True(actualValue);
        }
    }
}