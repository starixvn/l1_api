using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.Log;
using L1.Shared.Models;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.OWCommunicators.Log
{
    public class LogServiceTests
    {
        private readonly ILogDataAccess logDataAccess;

        public LogServiceTests()
        {
            logDataAccess = Substitute.For<ILogDataAccess>();
        }

        [Theory, AutoData]
        public async Task LogBetAsync_Always_WriteBetAsync(OWResponse<OWResult> response, byte system, OWAuth auth, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new OWRequest(auth, call);

            // Act
            await service.LogBetAsync(response, system);

            // Assert
            await logDataAccess.Received().WriteBetAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task LogEndGameAsync_Always_WWriteEndGameAsync(OWResponse<OWResult> response, OWAuth auth, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new OWRequest(auth, call);

            // Act
            await service.LogEndGameAsync(response);

            // Assert
            await logDataAccess.Received().WriteEndGameAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task LogVoidGameAsync_Always_WriteVoidGameAsync(OWResponse<OWResult> response, OWAuth auth, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new OWRequest(auth, call);

            // Act
            await service.LogVoidGameAsync(response);

            // Assert
            await logDataAccess.Received().WriteVoidGameAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task LogFailedRequestAsync_Always_WriteFailedRequestAsync(OWResponse<OWResult> response, OWAuth auth, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new OWRequest(auth, call);

            // Act
            await service.LogFailedRequestAsync(response);

            // Assert
            await logDataAccess.Received().WriteFailedRequestAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task LogSignInAsync_Always_WriteSignInAsync(Request request, VerificationResult result)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            var log = new SBLog(request, result, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            await service.LogSignInAsync(log);

            // Assert
            await logDataAccess.Received().WriteSignInAsync(Arg.Any<SBLog>());
        }

        [Theory, AutoData]
        public async Task LogRetrieveEndGameAsync_Always_WriteRetrieveEndGameAsync(Request request, VerificationResult result)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            var log = new SBLog(request, result, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            await service.LogRetrieveEndGameAsync(log);

            // Assert
            await logDataAccess.Received().WriteRetrieveEndGameAsync(Arg.Any<SBLog>());
        }
    }
}