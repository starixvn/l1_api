using AutoFixture.Xunit2;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using System.Threading.Tasks;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWFailureHandlerTests
    {
        private readonly ILogService logService;
        private readonly IOWMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings = Substitute.For<IAppSettings>();

        private readonly OWFailureHandler owFailureHandler;

        public OWFailureHandlerTests()
        {
            logService = Substitute.For<ILogService>();
            maintenanceService = Substitute.For<IOWMaintenanceService>();
            sentryClient = Substitute.For<ISentryClient>();
            appSettings.PartnerIds = new PartnerIds
            {
                OW = "OW",
                SBO = "SBO"
            };

            owFailureHandler = new OWFailureHandler(logService, maintenanceService, sentryClient);
        }

        [Theory, AutoData]
        public async Task Handle_Always_LogFailedRequest(PartnerResult result)
        {
            // Arrange

            // Act
            await owFailureHandler.Handle(new PartnerResponse<PartnerResult> { Result = result }, "OW");

            // Assert
            await logService.Received(1).LogFailedRequestAsync(appSettings.PartnerIds.OW, Arg.Any<PartnerResponse<PartnerResult>>());
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
            await owFailureHandler.Handle(response, "OW");

            // Assert
            await maintenanceService.Received(1).ObserveMaintenanceSchedule(Arg.Any<OWRequest>(), "OW");
        }
    }
}