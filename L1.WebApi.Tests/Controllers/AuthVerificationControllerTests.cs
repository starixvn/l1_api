using System.Threading.Tasks;
using AutoFixture;
using L1.Features.MemberAuthentications;
using L1.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace L1.WebApi.Tests.Controllers
{
    public class AuthVerificationControllerTests
    {
        private readonly IMemberAuthService memberAuthService;
        private readonly AuthVerificationController controller;
        private readonly Fixture fixture = new Fixture();

        public AuthVerificationControllerTests()
        {
            memberAuthService = Substitute.For<IMemberAuthService>();
            controller = new AuthVerificationController(memberAuthService);
        }

        [Fact]
        public async Task Verify_Always_ReturnResultFromMemberAuthService()
        {
            // Arrange
            var verificationParams = fixture.Create<VerificationParams>();
            var expectedResult = fixture.Create<VerificationResult>();
            memberAuthService.VerifyToken(Arg.Any<VerificationParams>()).Returns(expectedResult);

            // Act
            var actualValue = await controller.Verify(verificationParams);
            var objectResult = actualValue.Result as OkObjectResult; // ✅ Lấy OkObjectResult
            var responseValue = objectResult?.Value as VerificationResult; // ✅ Giải nén giá trị

            // Assert
            Assert.NotNull(objectResult);
            Assert.NotNull(responseValue);
            Assert.Equal(expectedResult, responseValue);
        }
    }
}