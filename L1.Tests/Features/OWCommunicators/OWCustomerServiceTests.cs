using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.GetBetLimit;
using L1.Features.OWCommunicators.Log;
using L1.Shared.Configurations;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWCustomerServiceTests
    {
        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithEnterPortalEndpoint(EnterPortalCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();           
            var logService = Substitute.For<ILogService>();
            var appSettings = Substitute.For<IAppSettings>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var owService = Substitute.For<IOWService>();


            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            await service.EnterPortal(call);

            // Assert
            await owService.Received().Post<EnterPortalResult>(endpoints.EnterPortal, call);
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithBalanceEndpoint(GetBalanceCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var appSettings = Substitute.For<IAppSettings>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var owService = Substitute.For<IOWService>();


            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            await service.GetBalance(call);

            // Assert
            await owService.Received().Post<GetBalanceResult>(endpoints.GetBalance, call);
        }

        [Theory, AutoData]
        public async Task GetBetLimit_ShouldPostWithBalanceEndpoint(GetBetLimitCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var appSettings = Substitute.For<IAppSettings>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var owService = Substitute.For<IOWService>();


            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            await service.GetBetLimit(call);

            // Assert
            await owService.Received().Post<GetBetLimitResult>(endpoints.GetBetLimit, call);
        }
    }
}