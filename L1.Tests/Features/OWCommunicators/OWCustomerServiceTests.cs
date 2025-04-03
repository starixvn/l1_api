using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWCustomerServiceTests
    {
        private readonly OWEndPoints endpoints = Substitute.For<OWEndPoints>();
        private readonly ILogService logService = Substitute.For<ILogService>();
        private readonly IAppSettings appSettings = Substitute.For<IAppSettings>();
        private readonly IAdminApiService adminApiSerivce = Substitute.For<IAdminApiService>();
        private readonly IOWService owService = Substitute.For<IOWService>();
        private readonly ISentryClient sentryClient = Substitute.For<ISentryClient>();
        private readonly IPartnerEndGameHandler partnerEndGameHandler = Substitute.For<IPartnerEndGameHandler>();

        private readonly OWCustomerService service;

        public OWCustomerServiceTests()
        {
            service = new OWCustomerService(owService, logService, appSettings, adminApiSerivce, endpoints, sentryClient, partnerEndGameHandler);
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO"
            };
            appSettings.LogEnabling.Returns(new LogEnabling { EnterPortal = true, GetBetLimit = true });
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithEnterPortalEndpoint(PartnerEnterPortalCall call)
        {
            // Arrange
            service.OperatorId = "OW";
            var response = new PartnerResponse<PartnerEnterPortalResult> { Result = new PartnerEnterPortalResult() };
            owService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call, "OW")
             .Returns(x => Task.FromResult(response));
            // Act
            await service.EnterPortal(call);

            // Assert
            await owService.Received().Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call, "OW");
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldPostWithBalanceEndpoint(PartnerGetBalanceCall call)
        {
            // Arrange
            service.OperatorId = "OW";

            // Act
            await service.GetBalance(call);

            // Assert
            await owService.Received().Post<PartnerGetBalanceResult>(endpoints.GetBalance, call, "OW");
        }

        [Theory, AutoData]
        public async Task GetBetLimit_ShouldPostWithBetLimitEndpoint(PartnerGetBetLimitCall call)
        {
            // Arrange
            service.OperatorId = "OW";
            var response = new PartnerResponse<PartnerGetBetLimitResult> { Result = new PartnerGetBetLimitResult() };
            owService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call, "OW")
             .Returns(x => Task.FromResult(response));
            // Act
            await service.GetBetLimit(call);

            // Assert
            await owService.Received().Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call, "OW");
        }

        [Theory, AutoData]
        public async Task PlaceBet_SuccessfulResponse_LogBet(PartnerPlaceBetCall call)
        {
            // Arrange
            service.OperatorId = "OW";
            var response = new PartnerResponse<PartnerPlaceBetResult> { Result = new PartnerPlaceBetResult { Balance = 2000 } };
            owService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call, "OW")
                .Returns(x => Task.FromResult(response));

            // Act
            await service.PlaceBet(call, 1);

            // Assert
            await logService.Received(1).LogBetAsync(appSettings.PartnerIds.OW, response, 1);
        }

        [Theory, AutoData]
        public async Task PlaceBet_HighStake_ShouldNotify(PartnerPlaceBetCall call)
        {
            // Arrange
            service.OperatorId = "OW";
            appSettings.HighStake.Returns(500);
            call.Amount = appSettings.HighStake;
            var response = new PartnerResponse<PartnerPlaceBetResult> { Result = new PartnerPlaceBetResult { Balance = 2000 } };
            owService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call, "OW").Returns(x => Task.FromResult(response));

            // Act
            await service.PlaceBet(call, 1);

            // Assert
            await adminApiSerivce.Received(1).NotifyHighStake(Arg.Any<NotifyHighStakeRequest>());
        }

        [Theory, AutoData]
        public async Task EndGame_SuccessfulResponse_LogEndGame(PartnerEndGameCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerEndGameResult> { Result = new PartnerEndGameResult() };
            owService.Post<PartnerEndGameResult>(endpoints.EndGame, call, "OW")
                .Returns(x => Task.FromResult(response));
            service.OperatorId = "OW";

            // Act
            await service.EndGame(call);

            // Assert
            await owService.Received(1).Post<PartnerEndGameResult>(endpoints.EndGame, call, "OW");
            await logService.Received(1).LogEndGameAsync(appSettings.PartnerIds.OW, response);
        }

        [Theory, AutoData]
        public async Task EndGame_SuccessfulResponse_Returns(PartnerEndGameCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerEndGameResult> { Result = new PartnerEndGameResult() };
            owService.Post<PartnerEndGameResult>(endpoints.EndGame, call, "OW")
                .Returns(x => Task.FromResult(response));
            service.OperatorId = "OW";

            // Act
            var actualReponse = await service.EndGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }

        [Theory, AutoData]
        public async Task VoidGame_SuccessfulResponse_LogVoidGame(PartnerVoidGameCall call)
        {
            // Arrange
            service.OperatorId = "OW";
            var response = new PartnerResponse<PartnerVoidGameResult> { Result = new PartnerVoidGameResult() };
            owService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call, "OW")
                .Returns(x => Task.FromResult(response));

            // Act
            await service.VoidGame(call);

            // Assert
            await owService.Received(1).Post<PartnerVoidGameResult>(endpoints.VoidGame, call, "OW");
            await logService.Received(1).LogVoidGameAsync(appSettings.PartnerIds.OW, response);
        }

        [Theory, AutoData]
        public async Task VoidGame_SuccessfulResponse_Returns(PartnerVoidGameCall call)
        {
            // Arrange
            service.OperatorId = "OW";
            var response = new PartnerResponse<PartnerVoidGameResult> { Result = new PartnerVoidGameResult() };
            owService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call, "OW")
                .Returns(x => Task.FromResult(response));

            // Act
            var actualReponse = await service.VoidGame(call);

            // Assert
            Assert.Equal(response, actualReponse);
        }
    }
}