using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.Resources;
using L1.Features.SGCommunicators;
using L1.Features.SGCommunicators.CheckMaintenance;
using L1.Shared.Constants;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.SGCommunicators
{
    public class SGFailureHandlerTests
    {
        private readonly ILogService logService;
        private readonly ISGMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly SGFailureHandler failureHandler;
        private const string operatorId = "SG";

        public SGFailureHandlerTests()
        {
            logService = Substitute.For<ILogService>();
            maintenanceService = Substitute.For<ISGMaintenanceService>();
            sentryClient = Substitute.For<ISentryClient>();
            failureHandler = new SGFailureHandler(logService, maintenanceService, sentryClient);
        }

        [Theory, AutoData]
        public async Task Handle_Always_LogsFailedRequest(PartnerResult result)
        {
            // Arrange
            var response = new PartnerResponse<PartnerResult> { Result = result };

            // Act
            await failureHandler.Handle(response, operatorId);

            // Assert
            await logService.Received(1).LogFailedRequestAsync(operatorId, response);
        }

        [Theory, AutoData]
        public async Task Handle_Always_CapturesSentryMessage(PartnerResult result)
        {
            // Arrange
            var response = new PartnerResponse<PartnerResult> { Result = result };
            var expectedMessage = $"Request to {operatorId} failed at {DateTimeOffset.UtcNow} - {result.ErrorDescription}";

            // Act
            await failureHandler.Handle(response, operatorId);

            // Assert
#pragma warning disable NS1001 // Non-virtual setup specification.
            // this is SentryExtensions
            sentryClient.Received(1).CaptureMessage(expectedMessage, SentryLevel.Warning);
#pragma warning restore NS1001 // Non-virtual setup specification.
        }

        [Theory, AutoData]
        public async Task Handle_WhenUnderMaintenance_ObservesMaintenanceSchedule(PartnerCall call)
        {
            // Arrange
            var response = new PartnerResponse<PartnerResult>
            {
                Result = new PartnerResult { ErrorCode = PartnerSpecialErrorCodes.UnderMaintenance },
                Request = new PartnerRequest(call)
            };

            // Act
            await failureHandler.Handle(response, operatorId);

            // Assert
            await maintenanceService.Received(1).ObserveMaintenanceSchedule(
                Arg.Is<SGRequest>(r => r.Call == call),
                operatorId);
        }

        [Theory]
        [InlineData(PartnerSpecialErrorCodes.NetworkError)]
        public async Task Handle_WhenNotUnderMaintenance_DoesNotObserveMaintenanceSchedule(int errorCode)
        {
            // Arrange
            var response = new PartnerResponse<PartnerResult>
            {
                Result = new PartnerResult { ErrorCode = errorCode }
            };

            // Act
            await failureHandler.Handle(response, operatorId);

            // Assert
            await maintenanceService.DidNotReceive().ObserveMaintenanceSchedule(
                Arg.Any<SGRequest>(),
                Arg.Any<string>());
        }

        [Theory]
        [InlineData(2003, "An error has occurred. Please contact CSD for details, thank you.")]
        [InlineData(3002, "Your account has Insufficient funds. ")]
        public async Task Handle_MapsErrorMessage_FromResourceManager(int errorCode, string expectedMessage)
        {
            // Arrange
            var result = new PartnerResult { ErrorCode = errorCode };
            var response = new PartnerResponse<PartnerResult> { Result = result };

            // Act
            await failureHandler.Handle(response, operatorId);

            // Assert
            Assert.Equal(expectedMessage, result.ErrorMessage);
        }

        [Theory, AutoData]
        public async Task Handle_WhenErrorCodeNotFound_UsesInternalError(int unknownErrorCode)
        {
            // Arrange
            var result = new PartnerResult { ErrorCode = unknownErrorCode };
            var response = new PartnerResponse<PartnerResult> { Result = result };

            // Act
            await failureHandler.Handle(response, operatorId);

            // Assert
            Assert.Equal(
                PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError),
                result.ErrorMessage);
        }
    }
}