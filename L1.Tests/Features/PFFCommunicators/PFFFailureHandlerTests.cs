using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PFFCommunicators;
using L1.Features.PFFCommunicators.CheckMaintenance;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using System.Threading.Tasks;
using Xunit;

namespace L1.Tests.Features.PFFCommunicators
{
    public class PFFFailureHandlerTests
    {
        private readonly ILogService logService;
        private readonly IPFFMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        private readonly PFFFailureHandler owFailureHandler;

        public PFFFailureHandlerTests()
        {
            logService = Substitute.For<ILogService>();
            maintenanceService = Substitute.For<IPFFMaintenanceService>();
            sentryClient = Substitute.For<ISentryClient>();
            appSettings = Substitute.For<IAppSettings>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO",
                PFF = "PFF"
            };

            owFailureHandler = new PFFFailureHandler(logService, maintenanceService, sentryClient, appSettings);
        }

        [Theory, AutoData]
        public async Task Handle_Always_LogFailedRequest(PartnerResult result)
        {
            // Arrange

            // Act
            await owFailureHandler.Handle(new PartnerResponse<PartnerResult> { Result = result });

            // Assert
            await logService.Received(1).LogFailedRequestAsync(appSettings.PartnerIds.PFF, Arg.Any<PartnerResponse<PartnerResult>>());
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
            await owFailureHandler.Handle(response);

            // Assert
            await maintenanceService.Received(1).ObserveMaintenanceSchedule(Arg.Any<PFFRequest>());
        }
    }
}