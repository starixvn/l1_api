using System.Threading.Tasks;
using AutoFixture;
using L1.Features.MemberAuthentications;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.Partners;
using L1.Shared.Constants;
using L1.WebApi.Controllers;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace L1.WebApi.Tests.Controllers
{
    public class SignInControllerTests
    {
        private readonly IMemberAuthService memberAuthService;
        private readonly IPartnerDataService partnerService;
        private readonly ILogService logService;
        private readonly IPartnerRequestAuthentication partnerRequestAuthentication;
        private readonly SignInController controller;
        private readonly Fixture fixture = new();

        public SignInControllerTests()
        {
            memberAuthService = Substitute.For<IMemberAuthService>();
            partnerService = Substitute.For<IPartnerDataService>();
            logService = Substitute.For<ILogService>();
            partnerRequestAuthentication = Substitute.For<IPartnerRequestAuthentication>();

            controller = new SignInController(memberAuthService, partnerService, logService, partnerRequestAuthentication);
        }

        [Fact]
        public async Task Index_InvalidClient_ReturnUnauthorizedResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.Auth = null;
            partnerService.VerifyWithCurrency(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            // Act
            var actualValue = await controller.Index(model) as UnauthorizedResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async Task Index_ValidClientWithEmptyIP_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.IP = string.Empty;
            partnerService.VerifyWithCurrency(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            partnerRequestAuthentication.Verify(Arg.Any<string>(), Arg.Any<HttpRequest>()).Returns(true);

            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.SignIn(Arg.Any<MemberAuth>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.Index(model) as OkObjectResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async Task Index_ValidClientWithInvalidAppType_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.IP = "IP";
            model.Auth = new Auth { OperatorId = "VG" };
            model.AppType = 0;
            partnerService.VerifyWithCurrency(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            partnerRequestAuthentication.Verify(Arg.Any<string>(), Arg.Any<HttpRequest>()).Returns(true);

            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.SignIn(Arg.Any<MemberAuth>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.Index(model) as OkObjectResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async Task Index_ValidClientWithValidAppType_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.IP = "IP";
            model.Auth = new Auth();
            model.AppType = AppTypes.SabaClub;
            partnerService.VerifyWithCurrency(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            partnerRequestAuthentication.Verify(Arg.Any<string>(), Arg.Any<HttpRequest>()).Returns(true);

            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.SignIn(Arg.Any<MemberAuth>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.Index(model) as OkObjectResult;

            // Assert
            Assert.NotNull(actualValue);
        }
    }
}