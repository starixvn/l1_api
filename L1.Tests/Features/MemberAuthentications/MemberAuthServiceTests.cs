using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.MemberAuthentications;
using L1.Shared.Configurations;
using L1.Shared.Json;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.MemberAuthentications
{
    public class MemberAuthServiceTests
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;

        private readonly MemberAuthService memberAuthService;

        public MemberAuthServiceTests()
        {
            memberAuthDataAccess = Substitute.For<IMemberAuthDataAccess>();
            appSettings = Substitute.For<IAppSettings>();
            sentryClient = Substitute.For<ISentryClient>();
            jsonService = Substitute.For<IJsonService>();
            memberAuthService = new MemberAuthService(memberAuthDataAccess, appSettings, sentryClient, jsonService);
        }

        [Theory, AutoData]
        public async Task VerifyToken_NotFoundMemberAuth_ReturnsError(VerificationParams verificationParams)
        {
            // Arrange

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.Equal(VerificationResult.TokenNotFoundCode, result.Code);
        }

        [Theory, AutoData]
        public async Task VerifyToken_HasMemberAuthButDifferentBrowserUserAgent_ReturnsError(VerificationParams verificationParams)
        {
            // Arrange
            var token = "token";
            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(new MemberAuth
            {
                Token = token,
                Time = DateTime.UtcNow
            });

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.Equal(VerificationResult.InvalidUserAgentOrIPCode, result.Code);
        }

        [Theory, AutoData]
        public async Task VerifyToken_HasMemberAuthButDifferentBrowserUserAgent2_ReturnsError(VerificationParams verificationParams)
        {
            // Arrange
            var token = "token";
            verificationParams.BrowserUserAgent = null;
            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(new MemberAuth
            {
                Token = token,
                Time = DateTime.UtcNow,
                BrowserUserAgent = "BrowserUserAgent"
            });

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.Equal(VerificationResult.InvalidUserAgentOrIPCode, result.Code);
        }

        [Theory, AutoData]
        public async Task VerifyToken_ExpiredToken_ReturnsError(VerificationParams verificationParams)
        {
            // Arrange
            var token = "token";
            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(new MemberAuth
            {
                Token = token,
                Time = DateTime.UtcNow.AddSeconds(-70),
                BrowserUserAgent = verificationParams.BrowserUserAgent
            });
            appSettings.TokenExpiredSeconds.Returns(60);

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.Equal(VerificationResult.TokenExpiredCode, result.Code);
        }

        [Theory, AutoData]
        public async Task VerifyToken_Success_ReturnsResultWithMemberAuth(VerificationParams verificationParams)
        {
            // Arrange
            var memberAuth = new MemberAuth
            {
                Token = "token",
                Time = DateTime.UtcNow,
                BrowserUserAgent = verificationParams.BrowserUserAgent,
                PlayerId = "1",
                PlayerName = "name",
                SiteId = "3332",
                OperatorId = "partner",
                CurrencyName = "ISA"
            };

            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(memberAuth);
            appSettings.TokenExpiredSeconds.Returns(60);

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.Equal(memberAuth.PlayerId, result.PlayerId);
            Assert.Equal(memberAuth.PlayerName, result.PlayerName);
            Assert.Equal(memberAuth.SiteId, result.SiteId);
        }

        [Theory, AutoData]
        public async Task VerifyToken_TokenNotExpired_ReturnsSuccess(VerificationParams verificationParams)
        {
            // Arrange
            var token = "token";
            var currentTime = DateTime.UtcNow;
            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(new MemberAuth
            {
                Token = token,
                Time = currentTime,
                BrowserUserAgent = verificationParams.BrowserUserAgent,
                PlayerId = "1"
            });
            appSettings.TokenExpiredSeconds.Returns(60);

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.NotEqual(VerificationResult.TokenExpiredCode, result.Code);
        }

        [Theory, AutoData]
        public async Task VerifyToken_TokenJustExpired_ReturnsError(VerificationParams verificationParams)
        {
            // Arrange
            var token = "token";
            var expirationSeconds = 60;
            var currentTime = DateTime.UtcNow;
            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(new MemberAuth
            {
                Token = token,
                Time = currentTime.AddSeconds(-expirationSeconds),
                BrowserUserAgent = verificationParams.BrowserUserAgent
            });
            appSettings.TokenExpiredSeconds.Returns(expirationSeconds);

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.Equal(VerificationResult.TokenExpiredCode, result.Code);
        }
    }
}