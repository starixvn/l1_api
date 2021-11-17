using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.SboCommunicators;
using L1.Features.SboCommunicators.CheckMaintenance;
using L1.Shared.Constants;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.SboCommunicators
{
    public class SboFailureHandlerTests
    {
        private readonly ILogService logService;
        private readonly ISboMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;

        private readonly SboFailureHandler owFailureHandler;

        public SboFailureHandlerTests()
        {
            logService = Substitute.For<ILogService>();
            maintenanceService = Substitute.For<ISboMaintenanceService>();
            sentryClient = Substitute.For<ISentryClient>();

            owFailureHandler = new SboFailureHandler(logService, maintenanceService, sentryClient);
        }

        [Theory, AutoData]
        public async Task Handle_Always_LogFailedRequest(PartnerResult result)
        {
            // Arrange

            // Act
            await owFailureHandler.Handle(new PartnerResponse<PartnerResult> { Result = result });

            // Assert
            await logService.Received(1).LogFailedRequestAsync(PartnerIds.SBO, Arg.Any<PartnerResponse<PartnerResult>>());
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
            await maintenanceService.Received(1).ObserveMaintenanceSchedule(Arg.Any<SboRequest>());
        }
    }
}