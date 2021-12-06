using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using L1.Features.MemberAuthentications;
using L1.Features.Partners;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.MemberAuthentications
{
    public class MemberAuthServiceTests
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly IPartnerDataService partnerDataService;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;
        private readonly Fixture fixture = new Fixture();

        private readonly MemberAuthService memberAuthService;

        public MemberAuthServiceTests()
        {
            memberAuthDataAccess = Substitute.For<IMemberAuthDataAccess>();
            partnerDataService = Substitute.For<IPartnerDataService>();
            appSettings = Substitute.For<IAppSettings>();
            sentryClient = Substitute.For<ISentryClient>();

            memberAuthService = new MemberAuthService(memberAuthDataAccess, partnerDataService, appSettings, sentryClient);
        }

        [Theory, AutoData]
        public async Task SignIn_NotFoundClientUrl_ReturnsGameServerNotFoundCode(MemberAuth memberAuth)
        {
            // Arrange
            partnerDataService.GetCurrency(memberAuth.PartnerId, memberAuth.CurrencyName).Returns(new Currency());

            // Act
            var result = await memberAuthService.SignIn(memberAuth, null);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Equal(result.Code, SingleSignOnResult.GameServerNotFoundCode);
        }

        [Theory]
        [InlineData(AppTypes.MiniSabaClub)]
        [InlineData(AppTypes.SabaMobi)]
        [InlineData(100)]
        public async Task SignIn_Success_ReturnsSingleSignOnResult(byte appType)
        {
            // Arrange
            var memberAuth = fixture.Create<MemberAuth>();
            memberAuth.AppType = appType;

            partnerDataService.GetById(memberAuth.PartnerId).Returns(new Partner
            {
                ClubVersionUrl = "http://local/",
                MobiVersionUrl = "http://mobi/",
                MiniVersionUrl = "http://mini/"
            });

            // Act
            var result = await memberAuthService.SignIn(memberAuth, null);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
        }

        [Theory, AutoData]
        public async Task SignIn_WithGameId_ReturnsRedirectUrlContainsGameId(MemberAuth memberAuth)
        {
            // Arrange
            byte? gameTypeId = 1;
            partnerDataService.GetById(memberAuth.PartnerId).Returns(new Partner
            {
                ClubVersionUrl = "http://local/",
                MobiVersionUrl = "http://mobi/",
                MiniVersionUrl = "http://mini/"
            });
            partnerDataService.GetCurrency(memberAuth.PartnerId, memberAuth.CurrencyName).Returns(new Currency());

            // Act
            var result = await memberAuthService.SignIn(memberAuth, gameTypeId);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Contains($"gameId={gameTypeId.Value}", result.RedirectUrl);
        }

        [Theory, AutoData]
        public async Task SignIn_TestCurrency_ReturnsRedirectUrlContainsPartnerId(MemberAuth memberAuth)
        {
            // Arrange
            partnerDataService.GetById(memberAuth.PartnerId).Returns(new Partner
            {
                ClubVersionUrl = "http://local/",
                MobiVersionUrl = "http://mobi/",
                MiniVersionUrl = "http://mini/",
                TestCurrencyUrl = "http://test/"
            });
            partnerDataService.GetCurrency(memberAuth.PartnerId, memberAuth.CurrencyName).Returns(new Currency { IsTest = true });

            // Act
            var result = await memberAuthService.SignIn(memberAuth, null);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Contains($"partnerId={memberAuth.PartnerId}", result.RedirectUrl);
        }

        [Theory, AutoData]
        public async Task SignIn_ExistingToken_ShouldBeReUse(MemberAuth memberAuth)
        {
            // Arrange
            var token = "token";
            partnerDataService.GetById(memberAuth.PartnerId).Returns(new Partner
            {
                ClubVersionUrl = "http://local/",
                MobiVersionUrl = "http://mobi/",
                MiniVersionUrl = "http://mini/"
            });
            memberAuthDataAccess.GetLatest(memberAuth).Returns(new MemberAuth
            {
                Token = token,
                Time = DateTime.Now
            });
            partnerDataService.GetCurrency(memberAuth.PartnerId, memberAuth.CurrencyName).Returns(new Currency());
            appSettings.TokenExpiredSeconds.Returns(60);

            // Act
            var result = await memberAuthService.SignIn(memberAuth, null);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Contains($"token={token}", result.RedirectUrl);
        }

        [Theory, AutoData]
        public async Task SignIn_ExistingToken_ShouldContainsPartnerIdIfTestCurrency(MemberAuth memberAuth)
        {
            // Arrange
            var token = "token";
            var partnerId = "partnerId";
            partnerDataService.GetById(memberAuth.PartnerId).Returns(new Partner
            {
                ClubVersionUrl = "http://local/",
                MobiVersionUrl = "http://mobi/",
                MiniVersionUrl = "http://mini/",
                TestCurrencyUrl = "http://test/"
            });
            memberAuthDataAccess.GetLatest(memberAuth).Returns(new MemberAuth
            {
                Token = token,
                Time = DateTime.Now,
                PartnerId = partnerId,
                IsTestCurrency = true
            });
            partnerDataService.GetCurrency(memberAuth.PartnerId, memberAuth.CurrencyName).Returns(new Currency { IsTest = true });
            appSettings.TokenExpiredSeconds.Returns(60);

            // Act
            var result = await memberAuthService.SignIn(memberAuth, null);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Contains($"token={token}", result.RedirectUrl);
            Assert.Contains($"partnerId={partnerId}", result.RedirectUrl);
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
                MemberId = 1,
                MemberName = "name",
                SiteId = "3332",
                PartnerId = "partner",
                CurrencyName = "ISA"
            };

            memberAuthDataAccess.GetByToken(verificationParams.Token).Returns(memberAuth);
            partnerDataService.GetCurrency(memberAuth.PartnerId, memberAuth.CurrencyName).Returns(new Currency());
            appSettings.TokenExpiredSeconds.Returns(60);

            // Act
            var result = await memberAuthService.VerifyToken(verificationParams);

            // Assert
            Assert.IsType<VerificationResult>(result);
            Assert.Equal(memberAuth.MemberId, result.MemberId);
            Assert.Equal(memberAuth.MemberName, result.MemberName);
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
                MemberId = 1,
                MemberName = "name",
                SiteId = "3332",
                PartnerId = "partner",
                CurrencyName = "ISA"
            };

            memberAuthDataAccess.GetByToken(updateMemberAuthParams.Token).Returns(memberAuth);

            // Act
            var result = await memberAuthService.UpdateMemberAuth(updateMemberAuthParams);

            // Assert
            Assert.IsType<SingleSignOnResult>(result);
            Assert.Equal(result.Code, SingleSignOnResult.Unauthorize);
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
            Assert.Equal(result.Code, SingleSignOnResult.Unauthorize);
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
                MemberId = 1,
                MemberName = "name",
                SiteId = "3332",
                PartnerId = "partner",
                CurrencyName = "ISA",
                IsTestCurrency = true,
                Language = updateMemberAuthParams.Language
            };

            partnerDataService.GetById(memberAuth.PartnerId).Returns(new Partner
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
                MemberId = 1,
                MemberName = "name",
                SiteId = "3332",
                PartnerId = "partner",
                CurrencyName = "ISA",
                IsTestCurrency = true,
                Language = updateMemberAuthParams.Language
            };

            partnerDataService.GetById(memberAuth.PartnerId).Returns(new Partner
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
            Assert.Contains($"&gameId={updateMemberAuthParams.GameTypeId}", result.RedirectUrl);
        }
    }
}