using System;
using System.Net;
using System.Threading.Tasks;
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

        public MaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();
        }

        [Theory, AutoData]
        public async Task ObserveMaintenanceSchedule_CreateSchedule(OWAuth auth, OWCall call)
        {
            // Arrange
            var maintenanceResult = new MaintenanceResult
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMilliseconds(5)
            };

            var owServiceSettings = new OWServiceSettings
            {
                BaseUrl = new Uri("http://localhost/"),
                Endpoints = new OWEndPoints
                {
                    CheckMaintenance = "maintenance"
                }
            };
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(new OWResponse<MaintenanceResult> { Result = maintenanceResult }));
            var maintenanceService = new MaintenanceService(httpClient, owServiceSettings, adminApiService, sentryClient);

            // Act
            await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(auth, call));

            // Assert
        }
    }
}