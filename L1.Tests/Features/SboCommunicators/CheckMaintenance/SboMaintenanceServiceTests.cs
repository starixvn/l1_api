using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using L1.Features.AdminApiCommunicators;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.SboCommunicators;
using L1.Features.SboCommunicators.CheckMaintenance;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.SboCommunicators.CheckMaintenance
{
    public class SboMaintenanceServiceTests
    {
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly SboServiceSettings sboServiceSettings;
        private readonly Fixture fixture = new();

        public SboMaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();

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

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new SboResponse<MaintenanceResult> { Result = maintenanceResult }));

            // Act
            var maintenanceService = new SboMaintenanceService(httpClient, sboServiceSettings, adminApiService, sentryClient);

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

            var httpClient = MockHttpClient.Create(HttpStatusCode.NotFound, JsonConvert.SerializeObject(new SboResponse<MaintenanceResult> { Result = maintenanceResult }));
            var maintenanceService = new SboMaintenanceService(httpClient, sboServiceSettings, adminApiService, sentryClient);
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
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(500)
            };

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new SboResponse<MaintenanceResult> { Result = maintenanceResult }));
            var maintenanceService = new SboMaintenanceService(httpClient, sboServiceSettings, adminApiService, sentryClient);
            var call = fixture.Create<SboCall>();
            await maintenanceService.ObserveMaintenanceSchedule(new SboRequest(call));

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new SboRequest(call));

            // Assert
            Assert.True(maintenanceService.IsUnderMaintenance());
        }
    }
}