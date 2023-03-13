using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Features.PFFCommunicators;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.PFFCommunicators
{
    public class PFFCustomerServiceTests
    {
        private readonly PFFEndPoints endpoints = Substitute.For<PFFEndPoints>();
        private readonly ILogService logService = Substitute.For<ILogService>();
        private readonly IPFFService PFFService = Substitute.For<IPFFService>();
        private readonly ISentryClient sentryClient = Substitute.For<ISentryClient>();
        private readonly IAppSettings appSettings = Substitute.For<IAppSettings>();

        private readonly PFFCustomerService service;

        public PFFCustomerServiceTests()
        {
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO",
                PFF = "PFF"
            };
            service = new PFFCustomerService(PFFService, logService, endpoints, sentryClient, appSettings);
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithEnterPortalEndpoint(PartnerEnterPortalCall call)
        {
            // Arrange

            // Act
            await service.EnterPortal(call);

            // Assert
            await PFFService.Received().Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithBalanceEndpoint(PartnerGetBalanceCall call)
        {
            // Arrange

            // Act
            await service.GetBalance(call);

            // Assert
            await PFFService.Received().Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);
        }

        [Theory, AutoData]
        public async Task GetBetLimit_ShouldPostWithBetLimitEndpoint(PartnerGetBetLimitCall call)
        {
            // Arrange

            // Act
            await service.GetBetLimit(call);

            // Assert
            await PFFService.Received().Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);
        }

        [Theory, AutoData]
        public async Task PlaceBet_SuccessfulResponse_LogBet(PartnerPlaceBetCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerPlaceBetResult> { Result = new PartnerPlaceBetResult { Balance = 2000 } };
            PFFService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call)
                .Returns(x => Task.FromResult(response));

            // Act
            await service.PlaceBet(call, 1);

            // Assert
            await logService.Received(1).LogBetAsync(appSettings.PartnerIds.PFF, response, 1);
        }

        [Theory, AutoData]
        public async Task EndGame_SuccessfulResponse_LogEndGame(PartnerEndGameCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerEndGameResult> { Result = new PartnerEndGameResult() };
            PFFService.Post<PartnerEndGameResult>(endpoints.EndGame, call)
                .Returns(x => Task.FromResult(response));

            // Act
            await service.EndGame(call);

            // Assert
            await PFFService.Received(1).Post<PartnerEndGameResult>(endpoints.EndGame, call);
            await logService.Received(1).LogEndGameAsync(appSettings.PartnerIds.PFF, response);
        }

        [Theory, AutoData]
        public async Task EndGame_SuccessfulResponse_Returns(PartnerEndGameCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerEndGameResult> { Result = new PartnerEndGameResult() };
            PFFService.Post<PartnerEndGameResult>(endpoints.EndGame, call)
                .Returns(x => Task.FromResult(response));

            // Act
            var actualReponse = await service.EndGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }

        [Theory, AutoData]
        public async Task VoidGame_SuccessfulResponse_LogVoidGame(PartnerVoidGameCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerVoidGameResult> { Result = new PartnerVoidGameResult() };
            PFFService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call)
                .Returns(x => Task.FromResult(response));

            // Act
            await service.VoidGame(call);

            // Assert
            await PFFService.Received(1).Post<PartnerVoidGameResult>(endpoints.VoidGame, call);
            await logService.Received(1).LogVoidGameAsync(appSettings.PartnerIds.PFF, response);
        }

        [Theory, AutoData]
        public async Task VoidGame_SuccessfulResponse_Returns(PartnerVoidGameCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerVoidGameResult> { Result = new PartnerVoidGameResult() };
            PFFService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call)
                .Returns(x => Task.FromResult(response));

            // Act
            var actualReponse = await service.VoidGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }
    }
}