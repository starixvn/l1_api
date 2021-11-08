using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
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
        public async Task LogBetAsync_Always_WriteBetAsync(string partnerId, PartnerResponse<PartnerResult> response, byte system, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new PartnerRequest(call);

            // Act
            await service.LogBetAsync(partnerId, response, system);

            // Assert
            await logDataAccess.Received().WriteBetAsync(Arg.Any<PartnerLog>());
        }

        [Theory, AutoData]
        public async Task LogEndGameAsync_Always_WWriteEndGameAsync(string partnerId, PartnerResponse<PartnerResult> response, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new PartnerRequest(call);

            // Act
            await service.LogEndGameAsync(partnerId, response);

            // Assert
            await logDataAccess.Received().WriteEndGameAsync(Arg.Any<PartnerLog>());
        }

        [Theory, AutoData]
        public async Task LogVoidGameAsync_Always_WriteVoidGameAsync(string partnerId, PartnerResponse<PartnerResult> response, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new PartnerRequest(call);

            // Act
            await service.LogVoidGameAsync(partnerId, response);

            // Assert
            await logDataAccess.Received().WriteVoidGameAsync(Arg.Any<PartnerLog>());
        }

        [Theory, AutoData]
        public async Task LogFailedRequestAsync_Always_WriteFailedRequestAsync(string partnerId, PartnerResponse<PartnerResult> response, OWCall call)
        {
            // Arrange
            var service = new LogService(logDataAccess);
            response.Request = new PartnerRequest(call);

            // Act
            await service.LogFailedRequestAsync(partnerId, response);

            // Assert
            await logDataAccess.Received().WriteFailedRequestAsync(Arg.Any<PartnerLog>());
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