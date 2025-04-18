using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.HotdogCommunicators;
using L1.Features.HotdogCommunicators.CheckMaintenance;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using System.Threading.Tasks;
using Xunit;

namespace L1.Tests.Features.HotdogCommunicators
{
    public class HotdogFailureHandlerTests
    {
        private readonly ILogService logService;
        private readonly IHotdogMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        private readonly HotdogFailureHandler owFailureHandler;

        public HotdogFailureHandlerTests()
        {
            logService = Substitute.For<ILogService>();
            maintenanceService = Substitute.For<IHotdogMaintenanceService>();
            sentryClient = Substitute.For<ISentryClient>();
            appSettings = Substitute.For<IAppSettings>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO",
                SabaPlay_HD = "Hotdog"
            };

            owFailureHandler = new HotdogFailureHandler(logService, maintenanceService, sentryClient, appSettings);
        }

        [Theory, AutoData]
        public async Task Handle_Always_LogFailedRequest(PartnerResult result)
        {
            // Arrange

            // Act
            await owFailureHandler.Handle(new PartnerResponse<PartnerResult> { Result = result }, appSettings.PartnerIds.SabaPlay_HD);

            // Assert
            await logService.Received(1).LogFailedRequestAsync(appSettings.PartnerIds.SabaPlay_HD, Arg.Any<PartnerResponse<PartnerResult>>());
        }

        [Theory, AutoData]
        public async Task Handle_UM_ShouldObserveSchedule(PartnerCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerResult>
            {
                Result = new PartnerResult { ErrorCode = PartnerSpecialErrorCodes.UnderMaintenance },
                Request = new PartnerRequest(call)
            };

            // Act
            await owFailureHandler.Handle(response, appSettings.PartnerIds.SabaPlay_HD);

            // Assert
            await maintenanceService.Received(1).ObserveMaintenanceSchedule(Arg.Any<HotdogRequest>(), appSettings.PartnerIds.SabaPlay_HD);
        }
    }
}