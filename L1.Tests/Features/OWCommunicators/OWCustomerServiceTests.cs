using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.GetBetLimit;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
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
        public async Task GetBetLimit_ShouldPostWithBetLimitEndpoint(GetBetLimitCall call)
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

        [Theory, AutoData]
        public async Task PlaceBet_SuccessfulResponse_LogBet(PlaceBetCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var appSettings = Substitute.For<IAppSettings>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var response = new OWResponse<PlaceBetResult> { Result = new PlaceBetResult { Balance = 2000 } };
            var owService = Substitute.For<IOWService>();
            owService.Post<PlaceBetResult>(endpoints.PlaceBet, call)
                .Returns(x => Task.FromResult(response));

            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            await service.PlaceBet(call, 1);

            // Assert
            await logService.Received(1).LogBetAsync(response, 1);
        }

        [Theory, AutoData]
        public async Task PlaceBet_HighStake_ShouldNotify(PlaceBetCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var owService = Substitute.For<IOWService>();
            var appSettings = Substitute.For<IAppSettings>();
            appSettings.HighStake.Returns(500);
            call.Amount = appSettings.HighStake;

            var response = new OWResponse<PlaceBetResult> { Result = new PlaceBetResult { Balance = 2000 } };
            owService.Post<PlaceBetResult>(endpoints.PlaceBet, call).Returns(x => Task.FromResult(response));
            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            await service.PlaceBet(call, 1);

            // Assert
            await adminApiSerivce.Received(1).NotifyHighStake(Arg.Any<NotifyHighStakeRequest>());
        }

        [Theory, AutoData]
        public async Task EndGame_SuccessfulResponse_LogEndGame(EndGameCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var appSettings = Substitute.For<IAppSettings>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var response = new OWResponse<EndGameResult> { Result = new EndGameResult () };
            var owService = Substitute.For<IOWService>();
            owService.Post<EndGameResult>(endpoints.EndGame, call)
                .Returns(x => Task.FromResult(response));

            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            await service.EndGame(call);

            // Assert
            await owService.Received(1).Post<EndGameResult>(endpoints.EndGame, call);
            await logService.Received(1).LogEndGameAsync(response);
        }

        [Theory, AutoData]
        public async Task EndGame_SuccessfulResponse_Returns(EndGameCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var appSettings = Substitute.For<IAppSettings>();
            var response = new OWResponse<EndGameResult> { Result = new EndGameResult() };
            var owService = Substitute.For<IOWService>();
            owService.Post<EndGameResult>(endpoints.EndGame, call)
                .Returns(x => Task.FromResult(response));

            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            var actualReponse = await service.EndGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }

        [Theory, AutoData]
        public async Task VoidGame_SuccessfulResponse_LogVoidGame(VoidGameCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var appSettings = Substitute.For<IAppSettings>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var response = new OWResponse<VoidGameResult> { Result = new VoidGameResult() };
            var owService = Substitute.For<IOWService>();
            owService.Post<VoidGameResult>(endpoints.VoidGame, call)
                .Returns(x => Task.FromResult(response));

            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            await service.VoidGame(call);

            // Assert
            await owService.Received(1).Post<VoidGameResult>(endpoints.VoidGame, call);
            await logService.Received(1).LogVoidGameAsync(response);
        }

        [Theory, AutoData]
        public async Task VoidGame_SuccessfulResponse_Returns(VoidGameCall call)
        {
            // Arrange
            var endpoints = Substitute.For<OWEndPoints>();
            var logService = Substitute.For<ILogService>();
            var adminApiSerivce = Substitute.For<IAdminApiService>();
            var appSettings = Substitute.For<IAppSettings>();
            var response = new OWResponse<VoidGameResult> { Result = new VoidGameResult() };
            var owService = Substitute.For<IOWService>();
            owService.Post<VoidGameResult>(endpoints.VoidGame, call)
                .Returns(x => Task.FromResult(response));

            var service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);

            // Act
            var actualReponse = await service.VoidGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }
    }
}