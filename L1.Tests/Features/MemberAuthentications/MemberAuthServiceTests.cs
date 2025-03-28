using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.GameConfigurations;
using L1.Features.MemberAuthentications;
using L1.Features.Partners;
using L1.Shared.Configurations;
using NSubstitute;
using Sentry;
using Xunit;
using L1.Shared.Json;

namespace L1.Tests.Features.MemberAuthentications
{
    public class MemberAuthServiceTests
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly IPartnerDataService partnerDataService;
        private readonly IConfigurationApiService configurationApiService;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;

        private readonly MemberAuthService memberAuthService;

        public MemberAuthServiceTests()
        {
            memberAuthDataAccess = Substitute.For<IMemberAuthDataAccess>();
            partnerDataService = Substitute.For<IPartnerDataService>();
            configurationApiService = Substitute.For<IConfigurationApiService>();
            appSettings = Substitute.For<IAppSettings>();
            sentryClient = Substitute.For<ISentryClient>();
            jsonService = Substitute.For<IJsonService>();
            memberAuthService = new MemberAuthService(memberAuthDataAccess, partnerDataService, configurationApiService, appSettings, sentryClient, jsonService);
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
                Time = DateTime.Now
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
                Time = DateTime.Now,
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
                Time = DateTime.Now.AddSeconds(-70),
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
                Time = DateTime.Now,
                BrowserUserAgent = verificationParams.BrowserUserAgent,
                PlayerId = "1",
                PlayerName = "name",
                SiteId = "3332",
                OperatorId = "partner",
                CurrencyName = "ISA"
            };

            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(memberAuth);
            partnerDataService.GetCurrency(memberAuth.OperatorId, memberAuth.CurrencyName).Returns(new Currency());
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
        public async Task UpdateMemberAuth_NotTestCurrency_ReturnsUnauthorize(UpdateMemberAuthParams updateMemberAuthParams)
        {
            // Arrange
            var memberAuth = new MemberAuth
            {
                Token = "token",
                Time = DateTime.Now,
                PlayerId = "1",
                PlayerName = "name",
                SiteId = "3332",
                OperatorId = "partner",
                CurrencyName = "ISA"
            };

            memberAuthDataAccess.GetByToken(updateMemberAuthParams.Token).Returns(memberAuth);

            // Act
            var result = await memberAuthService.UpdateMemberAuth(updateMemberAuthParams);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Equal(SingleSignOnResult.Unauthorize, result.Code);
        }

        [Theory, AutoData]
        public async Task UpdateMemberAuth_NotExist_ReturnsUnauthorize(UpdateMemberAuthParams updateMemberAuthParams)
        {
            // Arrange
            memberAuthDataAccess.GetByToken(updateMemberAuthParams.Token).Returns(default(MemberAuth));

            // Act
            var result = await memberAuthService.UpdateMemberAuth(updateMemberAuthParams);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Equal(SingleSignOnResult.Unauthorize, result.Code);
        }

        [Theory, AutoData]
        public async Task UpdateMemberAuth_WithoutGameId_ReturnsRedirectUrlNotContainsGameId(UpdateMemberAuthParams updateMemberAuthParams)
        {
            // Arrange
            updateMemberAuthParams.GameTypeId = null;
            var memberAuth = new MemberAuth
            {
                Token = updateMemberAuthParams.Token,
                Time = DateTime.Now,
                PlayerId = "1",
                PlayerName = "name",
                SiteId = "3332",
                OperatorId = "partner",
                CurrencyName = "ISA",
                IsTestCurrency = true,
                Language = updateMemberAuthParams.Language
            };

            partnerDataService.GetById(memberAuth.OperatorId).Returns(new Partner
            {
                ClubVersionUrl = "http://local/",
                MobiVersionUrl = "http://mobi/",
                MiniVersionUrl = "http://mini/"
            });

            memberAuthDataAccess.GetForTestCurrency(updateMemberAuthParams.Token).Returns(memberAuth);

            // Act
            var result = await memberAuthService.UpdateMemberAuth(updateMemberAuthParams);

            // Assert
            await memberAuthDataAccess.Received(1).UpdateCurrencyGroup(updateMemberAuthParams.Token, updateMemberAuthParams.CurrencyGroupId);
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Contains(updateMemberAuthParams.Token, result.RedirectUrl);
            Assert.Contains(updateMemberAuthParams.Language, result.RedirectUrl);
            Assert.DoesNotContain("gameId", result.RedirectUrl);
        }

        [Theory, AutoData]
        public async Task UpdateMemberAuth_WithGameId_ReturnsRedirectUrlContainsGameId(UpdateMemberAuthParams updateMemberAuthParams)
        {
            // Arrange
            updateMemberAuthParams.GameTypeId = 1;
            var memberAuth = new MemberAuth
            {
                Token = updateMemberAuthParams.Token,
                Time = DateTime.Now,
                PlayerId = "1",
                PlayerName = "name",
                SiteId = "3332",
                OperatorId = "partner",
                CurrencyName = "ISA",
                IsTestCurrency = true,
                Language = updateMemberAuthParams.Language
            };

            partnerDataService.GetById(memberAuth.OperatorId).Returns(new Partner
            {
                ClubVersionUrl = "http://local/",
                MobiVersionUrl = "http://mobi/",
                MiniVersionUrl = "http://mini/"
            });

            memberAuthDataAccess.GetForTestCurrency(updateMemberAuthParams.Token).Returns(memberAuth);
            configurationApiService.GetDetailGameConfiguration(1).Returns(new GameConfigurationResponse());

            // Act
            var result = await memberAuthService.UpdateMemberAuth(updateMemberAuthParams);

            // Assert
            await memberAuthDataAccess.Received(1).UpdateCurrencyGroup(updateMemberAuthParams.Token, updateMemberAuthParams.CurrencyGroupId);
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Contains(updateMemberAuthParams.Token, result.RedirectUrl);
            Assert.Contains(updateMemberAuthParams.Language, result.RedirectUrl);
            Assert.Contains($"&gameId={updateMemberAuthParams.GameTypeId}", result.RedirectUrl);
        }
    }
}