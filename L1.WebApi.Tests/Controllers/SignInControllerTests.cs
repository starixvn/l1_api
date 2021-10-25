using AutoFixture;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators.Log;
using L1.Features.Partners;
using L1.Shared.Constants;
using L1.WebApi.Controllers;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace L1.WebApi.Tests.Controllers
{
    public class SignInControllerTests
    {
        private readonly IMemberAuthService memberAuthService;
        private readonly IPartnerService partnerService;
        private readonly ILogService logService;
        private readonly SignInController controller;
        private readonly Fixture fixture = new Fixture();

        public SignInControllerTests()
        {
            memberAuthService = Substitute.For<IMemberAuthService>();
            partnerService = Substitute.For<IPartnerService>();
            logService = Substitute.For<ILogService>();

            controller = new SignInController(memberAuthService, partnerService, logService);
        }

        [Fact]
        public async void Index_InvalidClient_ReturnUnauthorizedResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.Auth = null;
            partnerService.Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            // Act
            var actualValue = await controller.Index(model) as UnauthorizedResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async void Index_ValidClientWithEmptyIP_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.IP = string.Empty;
            partnerService.Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.SignIn(Arg.Any<MemberAuth>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.Index(model) as OkObjectResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async void Index_ValidClientWithNullAuth_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.IP = "IP";
            model.Auth = null;
            partnerService.Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.SignIn(Arg.Any<MemberAuth>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.Index(model) as OkObjectResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async void Index_ValidClientWithInvalidAppType_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.IP = "IP";
            model.Auth = new Auth();
            model.AppType = 0;
            partnerService.Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.SignIn(Arg.Any<MemberAuth>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.Index(model) as OkObjectResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async void Index_ValidClientWithValidAppType_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<SignIn>();
            model.IP = "IP";
            model.Auth = new Auth();
            model.AppType = AppTypes.SabaClub;
            partnerService.Verify(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.SignIn(Arg.Any<MemberAuth>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.Index(model) as OkObjectResult;

            // Assert
            Assert.NotNull(actualValue);
        }
    }
}