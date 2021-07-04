using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.OWCommunicators.Log;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWFailureHandlerTests
    {
        private readonly ILogService logService;
        private readonly IMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;

        private readonly OWFailureHandler owFailureHandler;

        public OWFailureHandlerTests()
        {
            logService = Substitute.For<ILogService>();
            maintenanceService = Substitute.For<IMaintenanceService>();
            sentryClient = Substitute.For<ISentryClient>();

            owFailureHandler = new OWFailureHandler(logService, maintenanceService, sentryClient);
        }

        [Theory, AutoData]
        public async Task Handle_Always_LogFailedRequest(OWResult result)
        {
            // Arrange

            // Act
            await owFailureHandler.Handle(new OWResponse<OWResult> { Result = result });

            // Assert
            await logService.Received(1).LogFailedRequestAsync(Arg.Any<OWResponse<OWResult>>());
        }

        [Fact]
        public async Task Handle_UM_ShouldObserveSchedule()
        {
            // Arrange
            var response = new OWResponse<OWResult>
            {
                Result = new OWResult { ErrorCode = OWSpecialErrorCodes.UnderMaintenance }
            };

            // Act
            await owFailureHandler.Handle(response);

            // Assert
            await maintenanceService.Received(1).ObserveMaintenanceSchedule(Arg.Any<OWRequest>());
        }
    }
}