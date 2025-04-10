using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.SGCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Features.SGCommunicators.EnterPortal;
using L1.Features.SGCommunicators.PlaceBet;
using L1.Features.SGCommunicators.GetBalance;
using L1.Features.SGCommunicators.GetBetLimit;
using L1.Features.SGCommunicators.EndGame;
using L1.Features.SGCommunicators.VoidGame;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.SGCommunicators
{
    public class SGCustomerServiceTests
    {
        private readonly SGEndPoints endpoints;
        private readonly ILogService logService;
        private readonly IAppSettings appSettings;
        private readonly IAdminApiService adminApiService;
        private readonly ISGService sgService;
        private readonly ISentryClient sentryClient;
        private readonly INatsService natsService;
        private readonly SGCustomerService service;
        private const int TestPlayerId = 12345;

        public SGCustomerServiceTests()
        {
            endpoints = new SGEndPoints();
            logService = Substitute.For<ILogService>();
            appSettings = Substitute.For<IAppSettings>();
            adminApiService = Substitute.For<IAdminApiService>();
            sgService = Substitute.For<ISGService>();
            sentryClient = Substitute.For<ISentryClient>();
            natsService = Substitute.For<INatsService>();

            service = new SGCustomerService(
                sgService,
                natsService,
                logService,
                endpoints,
                sentryClient,
                appSettings);

            var partnerIds = new PartnerIds { SG = "SG" };
            appSettings.PartnerIds.Returns(partnerIds);
            appSettings.LogEnabling.Returns(new LogEnabling { EnterPortal = true, GetBetLimit = true });
            appSettings.HighStake.Returns(500);
            service.OperatorId = "SG";
        }

        [Theory, AutoData]
        public async Task EnterPortal_ShouldConvertPlayerIdToInteger(PartnerEnterPortalCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var response = new PartnerResponse<PartnerEnterPortalResult> 
            { 
                Result = new PartnerEnterPortalResult() 
            };

            sgService.Post<PartnerEnterPortalResult>(
                Arg.Is<string>(x => x == endpoints.EnterPortal),
                Arg.Any<SGEnterPortalCall>(),
                Arg.Is<string>(x => x == "SG"))
                .Returns(Task.FromResult(response));

            // Act
            await service.EnterPortal(call);

            // Assert
            await sgService.Received(1).Post<PartnerEnterPortalResult>(
                Arg.Is<string>(x => x == endpoints.EnterPortal),
                Arg.Is<SGEnterPortalCall>(x => x.PlayerId.ToString() == call.PlayerId.ToString()),
                Arg.Is<string>(x => x == "SG"));
        }

        [Theory, AutoData]
        public async Task PlaceBet_HighStake_ShouldNotifyAndPublish(PartnerPlaceBetCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            call.Amount = 1000m;
            var response = new PartnerResponse<PartnerPlaceBetResult>
            {
                Result = new PartnerPlaceBetResult { Balance = 2000 }
            };

            sgService.Post<PartnerPlaceBetResult>(
                Arg.Is<string>(x => x == endpoints.PlaceBet),
                Arg.Any<SGPlaceBetCall>(),
                Arg.Is<string>(x => x == "SG"))
                .Returns(Task.FromResult(response));

            // Act
            await service.PlaceBet(call);

            // Assert
            await natsService.Received(1).PublishAsync("bet.created", Arg.Any<object>());
            await logService.Received(1).LogBetAsync("SG", response, Arg.Any<byte>());
        }

        [Theory, AutoData]
        public async Task GetBalance_ShouldConvertPlayerIdToInteger(PartnerGetBalanceCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var response = new PartnerResponse<PartnerGetBalanceResult>
            {
                Result = new PartnerGetBalanceResult { Balance = 1000 }
            };

            sgService.Post<PartnerGetBalanceResult>(
                Arg.Is<string>(x => x == endpoints.GetBalance),
                Arg.Any<SGGetBalanceCall>(),
                Arg.Is<string>(x => x == "SG"))
                .Returns(Task.FromResult(response));

            // Act
            await service.GetBalance(call);

            // Assert
            await sgService.Received(1).Post<PartnerGetBalanceResult>(
                Arg.Is<string>(x => x == endpoints.GetBalance),
                Arg.Is<SGGetBalanceCall>(x => x.PlayerId.ToString() == call.PlayerId.ToString()),
                Arg.Is<string>(x => x == "SG"));
        }

        [Theory, AutoData]
        public async Task GetBetLimit_ShouldConvertPlayerIdToInteger(PartnerGetBetLimitCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var response = new PartnerResponse<PartnerGetBetLimitResult>
            {
                Result = new PartnerGetBetLimitResult()
            };

            sgService.Post<PartnerGetBetLimitResult>(
                Arg.Is<string>(x => x == endpoints.GetBetLimit),
                Arg.Any<SGGetBetLimitCall>(),
                Arg.Is<string>(x => x == "SG"))
                .Returns(Task.FromResult(response));

            // Act
            await service.GetBetLimit(call);

            // Assert
            await sgService.Received(1).Post<PartnerGetBetLimitResult>(
                Arg.Is<string>(x => x == endpoints.GetBetLimit),
                Arg.Is<SGGetBetLimitCall>(x => x.PlayerId.ToString() == call.PlayerId.ToString()),
                Arg.Is<string>(x => x == "SG"));
        }

        [Theory, AutoData]
        public async Task EndGame_ShouldConvertPlayerIdToInteger(PartnerEndGameCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var response = new PartnerResponse<PartnerEndGameResult>
            {
                Result = new PartnerEndGameResult()
            };

            sgService.Post<PartnerEndGameResult>(
                Arg.Is<string>(x => x == endpoints.EndGame),
                Arg.Any<SGEndGameCall>(),
                Arg.Is<string>(x => x == "SG"))
                .Returns(Task.FromResult(response));

            // Act
            await service.EndGame(call);

            // Assert
            await sgService.Received(1).Post<PartnerEndGameResult>(
                Arg.Is<string>(x => x == endpoints.EndGame),
                Arg.Is<SGEndGameCall>(x => x.PlayerId.ToString() == call.PlayerId.ToString()),
                Arg.Is<string>(x => x == "SG"));
        }

        [Theory, AutoData]
        public async Task VoidGame_ShouldConvertPlayerIdToInteger(PartnerVoidGameCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var response = new PartnerResponse<PartnerVoidGameResult>
            {
                Result = new PartnerVoidGameResult()
            };

            sgService.Post<PartnerVoidGameResult>(
                Arg.Is<string>(x => x == endpoints.VoidGame),
                Arg.Any<SGVoidGameCall>(),
                Arg.Is<string>(x => x == "SG"))
                .Returns(Task.FromResult(response));

            // Act
            await service.VoidGame(call);

            // Assert
            await sgService.Received(1).Post<PartnerVoidGameResult>(
                Arg.Is<string>(x => x == endpoints.VoidGame),
                Arg.Is<SGVoidGameCall>(x => x.PlayerId.ToString() == call.PlayerId.ToString()),
                Arg.Is<string>(x => x == "SG"));
        }
    }
} 