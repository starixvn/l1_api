using System;
using System.Collections.Generic;
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
        [Theory]
        [InlineData("partner", 1, "Mozilla/5.0", "Mozilla/5.0", true)]
        [InlineData("partner", 1, "Mozilla/5.0", "Chrome/1.0", false)]
        [InlineData("partner", 2, "Mozilla/5.0", "Mozilla/5.0", true)]
        [InlineData("partner", 2, "Mozilla/5.0", "Chrome/1.0", false)]
        public void VerifyUserAgent_BrowserUserAgentMatchOrNot_ReturnsExpectedResult(
            string operatorId, byte appType, string memberUserAgent, string paramUserAgent, bool shouldSucceed)
        {
            // Arrange
#pragma warning disable CA1861 // Avoid constant arrays as arguments
            var ignoreDict = new Dictionary<string, int[]>
            {
                { "other", new[] { 2 } }
            };
#pragma warning restore CA1861 // Avoid constant arrays as arguments
            appSettings.IgnoreBrowserAgentCheck.Returns(ignoreDict);

            var verificationParams = new VerificationParams
            {
                BrowserUserAgent = paramUserAgent
            };
            var memberAuth = new MemberAuth
            {
                OperatorId = operatorId,
                AppType = appType,
                BrowserUserAgent = memberUserAgent
            };

            // Act
            var result = typeof(MemberAuthService)
                .GetMethod("VerifyUserAgent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(memberAuthService, new object[] { verificationParams, memberAuth }) as VerificationResult;

            // Assert
            if (shouldSucceed)
            {
                Assert.Equal(VerificationResult.SuccessCode, result.Code);
            }
            else
            {
                Assert.Equal(VerificationResult.InvalidUserAgentOrIPCode, result.Code);
            }
        }

        [Fact]
        public void VerifyUserAgent_IgnoreBrowserAgentCheck_SkipsUserAgentCheck()
        {
            // Arrange
#pragma warning disable CA1861 // Avoid constant arrays as arguments
            var ignoreDict = new Dictionary<string, int[]>
            {
                { "partner", new[] { 5, 7 } }
            };
#pragma warning restore CA1861 // Avoid constant arrays as arguments
            appSettings.IgnoreBrowserAgentCheck.Returns(ignoreDict);

            var verificationParams = new VerificationParams
            {
                BrowserUserAgent = "SomeAgent"
            };
            var memberAuth = new MemberAuth
            {
                OperatorId = "partner",
                AppType = 7,
                BrowserUserAgent = "DifferentAgent"
            };

            // Act
            var result = typeof(MemberAuthService)
                .GetMethod("VerifyUserAgent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(memberAuthService, new object[] { verificationParams, memberAuth }) as VerificationResult;

            // Assert
            Assert.Equal(VerificationResult.SuccessCode, result.Code);
        }

        [Fact]
        public void VerifyUserAgent_IgnoreBrowserAgentCheck_NullDictionary_DoesNotSkip()
        {
            // Arrange
            appSettings.IgnoreBrowserAgentCheck.Returns((Dictionary<string, int[]>)null);

            var verificationParams = new VerificationParams
            {
                BrowserUserAgent = "UA1"
            };
            var memberAuth = new MemberAuth
            {
                OperatorId = "partner",
                AppType = 1,
                BrowserUserAgent = "UA2"
            };

            // Act
            var result = typeof(MemberAuthService)
                .GetMethod("VerifyUserAgent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(memberAuthService, new object[] { verificationParams, memberAuth }) as VerificationResult;

            // Assert
            Assert.Equal(VerificationResult.InvalidUserAgentOrIPCode, result.Code);
        }

        [Fact]
        public void VerifyUserAgent_IgnoreBrowserAgentCheck_EmptyAppTypes_DoesNotSkip()
        {
            // Arrange
            var ignoreDict = new Dictionary<string, int[]>
            {
                { "partner", null }
            };
            appSettings.IgnoreBrowserAgentCheck.Returns(ignoreDict);

            var verificationParams = new VerificationParams
            {
                BrowserUserAgent = "UA1"
            };
            var memberAuth = new MemberAuth
            {
                OperatorId = "partner",
                AppType = 1,
                BrowserUserAgent = "UA2"
            };

            // Act
            var result = typeof(MemberAuthService)
                .GetMethod("VerifyUserAgent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(memberAuthService, new object[] { verificationParams, memberAuth }) as VerificationResult;

            // Assert
            Assert.Equal(VerificationResult.InvalidUserAgentOrIPCode, result.Code);
        }


    }
}