using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Features.SGCommunicators;
using L1.Features.SGCommunicators.CheckMaintenance;
using L1.Shared.Helpers;
using L1.Shared.Json;
using L1.Tests._Shared;
using NSubstitute;
using Sentry;
using Xunit;
using L1.Features.AdminApiCommunicators.UnderMaintenance;

namespace L1.Tests.Features.SGCommunicators.CheckMaintenance
{
    public class SGMaintenanceServiceTests
    {
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;
        private readonly SGServiceSettings settings;
        private readonly PartnerMaintenanceResult defaultMaintenanceResult;
        private readonly PartnerResponse<PartnerMaintenanceResult> defaultResponse;
        private readonly string serializedResponse;
        private readonly SGRequest defaultRequest;
        private const string operatorId = "SG";

        public SGMaintenanceServiceTests()
        {
            adminApiService = Substitute.For<IAdminApiService>();
            sentryClient = Substitute.For<ISentryClient>();
            jsonService = Substitute.For<IJsonService>();
            settings = new SGServiceSettings
            {
                BaseUrl = new Uri("http://test.com"),
                SecretKey = "test-secret-key",
                Endpoints = new SGEndPoints
                {
                    CheckMaintenance = "maintenance"
                }
            };

            defaultMaintenanceResult = new PartnerMaintenanceResult
            {
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddSeconds(5)
            };

            defaultResponse = new PartnerResponse<PartnerMaintenanceResult>
            {
                Result = defaultMaintenanceResult
            };

            serializedResponse = "{\"Result\":{\"StartTime\":\"" + defaultMaintenanceResult.StartTime.Value.ToString("O") + 
                               "\",\"EndTime\":\"" + defaultMaintenanceResult.EndTime.Value.ToString("O") + "\"}}";

            defaultRequest = new SGRequest(new PartnerCall("123", "site1"));
            jsonService.Deserialize<PartnerResponse<PartnerMaintenanceResult>>(Arg.Any<string>()).Returns(defaultResponse);
            jsonService.Serialize(Arg.Any<PartnerCall>()).Returns("serialized-request");
        }

        private SGMaintenanceService CreateService(HttpClient httpClient = null)
        {
            httpClient ??= MockHttpClient.Create(HttpStatusCode.OK, serializedResponse);
            httpClient.BaseAddress = settings.BaseUrl;

            return new SGMaintenanceService(
                httpClient,
                settings,
                adminApiService,
                sentryClient,
                jsonService);
        }

        [Fact]
        public async Task ObserveMaintenanceSchedule_WhenMaintenanceIsActive_ShouldHandleMaintenanceSchedule()
        {
            // Arrange
            var service = CreateService();

            // Act
            await service.ObserveMaintenanceSchedule(defaultRequest, operatorId);

            // Assert
            await adminApiService.Received(1).UnderMaintenance(
                Arg.Is<MaintenanceRequest>(m =>
                    m.StartTime == defaultMaintenanceResult.StartTime &&
                    m.EndTime == defaultMaintenanceResult.EndTime &&
                    m.IsUM == defaultMaintenanceResult.IsUnderMaintenance() &&
                    m.PartnerId == operatorId));
        }

        [Fact]
        public async Task ObserveMaintenanceSchedule_WhenRequestFails_ShouldReturnEmptyResult()
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadRequest, "");
            var service = CreateService(httpClient);

            // Act
            await service.ObserveMaintenanceSchedule(defaultRequest, operatorId);

            // Assert
            await adminApiService.DidNotReceive().UnderMaintenance(Arg.Any<MaintenanceRequest>());
        }

        [Fact]
        public async Task ObserveMaintenanceSchedule_ShouldAddCorrectAccessKeyHeader()
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedResponse);
            var service = CreateService(httpClient);
            var expectedAccessKey = Md5Helper.Create($"{settings.SecretKey}serialized-request");

            // Act
            await service.ObserveMaintenanceSchedule(defaultRequest, operatorId);

            // Assert
            Assert.Contains("Saba-access-key", httpClient.DefaultRequestHeaders.Select(h => h.Key));
            Assert.Equal(expectedAccessKey, httpClient.DefaultRequestHeaders.GetValues("Saba-access-key").First());
        }

        [Fact]
        public async Task ObserveMaintenanceSchedule_ShouldUseCorrectEndpoint()
        {
            // Arrange
            var handler = new MockHttpMessageHandler
            {
                ResponseStatusCode = HttpStatusCode.OK,
                ResponseContent = serializedResponse
            };
            var httpClient = new HttpClient(handler) { BaseAddress = settings.BaseUrl };
            var service = CreateService(httpClient);

            // Act
            await service.ObserveMaintenanceSchedule(defaultRequest, operatorId);

            // Assert
            Assert.Equal(settings.Endpoints.CheckMaintenance, handler.RequestUri.AbsolutePath.TrimStart('/'));
        }
    }
} 