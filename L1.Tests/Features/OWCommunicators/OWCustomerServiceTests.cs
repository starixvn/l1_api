using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
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
        private readonly OWEndPoints endpoints = Substitute.For<OWEndPoints>();
        private readonly ILogService logService = Substitute.For<ILogService>();
        private readonly IOWService owService = Substitute.For<IOWService>();
        private readonly IAppSettings appSettings = Substitute.For<IAppSettings>();
        private readonly IAdminApiService adminApiSerivce = Substitute.For<IAdminApiService>();

        private readonly OWCustomerService service;

        public OWCustomerServiceTests()
        {
            service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints);
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithEnterPortalEndpoint(EnterPortalCall call)
        {
            // Arrange

            // Act
            await service.EnterPortal(call);

            // Assert
            await owService.Received().Post<EnterPortalResult>(endpoints.EnterPortal, call);
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithBalanceEndpoint(GetBalanceCall call)
        {
            // Arrange

            // Act
            await service.GetBalance(call);

            // Assert
            await owService.Received().Post<GetBalanceResult>(endpoints.GetBalance, call);
        }

        [Theory, AutoData]
        public async Task GetBetLimit_ShouldPostWithBetLimitEndpoint(GetBetLimitCall call)
        {
            // Arrange

            // Act
            await service.GetBetLimit(call);

            // Assert
            await owService.Received().Post<GetBetLimitResult>(endpoints.GetBetLimit, call);
        }

        [Theory, AutoData]
        public async Task PlaceBet_SuccessfulResponse_LogBet(PlaceBetCall call)
        {
            // Arrange
            var response = new OWResponse<PlaceBetResult> { Result = new PlaceBetResult { Balance = 2000 } };
            owService.Post<PlaceBetResult>(endpoints.PlaceBet, call)
                .Returns(x => Task.FromResult(response));

            // Act
            await service.PlaceBet(call, 1);

            // Assert
            await logService.Received(1).LogBetAsync(response, 1);
        }

        [Theory, AutoData]
        public async Task EndGame_SuccessfulResponse_LogEndGame(EndGameCall call)
        {
            // Arrange
            var response = new OWResponse<EndGameResult> { Result = new EndGameResult() };
            owService.Post<EndGameResult>(endpoints.EndGame, call)
                .Returns(x => Task.FromResult(response));

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
            var response = new OWResponse<EndGameResult> { Result = new EndGameResult() };
            owService.Post<EndGameResult>(endpoints.EndGame, call)
                .Returns(x => Task.FromResult(response));

            // Act
            var actualReponse = await service.EndGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }

        [Theory, AutoData]
        public async Task VoidGame_SuccessfulResponse_LogVoidGame(VoidGameCall call)
        {
            // Arrange
            var response = new OWResponse<VoidGameResult> { Result = new VoidGameResult() };
            owService.Post<VoidGameResult>(endpoints.VoidGame, call)
                .Returns(x => Task.FromResult(response));

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
            var response = new OWResponse<VoidGameResult> { Result = new VoidGameResult() };
            owService.Post<VoidGameResult>(endpoints.VoidGame, call)
                .Returns(x => Task.FromResult(response));

            // Act
            var actualReponse = await service.VoidGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }
    }
}