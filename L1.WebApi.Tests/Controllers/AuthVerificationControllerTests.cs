using System.Threading.Tasks;
using AutoFixture;
using L1.Features.MemberAuthentications;
using L1.WebApi.Controllers;
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
            var result = fixture.Create<VerificationResult>();
            memberAuthService.VerifyToken(Arg.Any<VerificationParams>()).Returns(result);

            // Act
            var actualValue = await controller.Verify(verificationParams);

            // Assert
            Assert.Equal(result, actualValue);
        }

        [Fact]
        public async Task UpdateMemberAuth_Always_ReturnResultFromMemberAuthService()
        {
            // Arrange
            var updateParams = fixture.Create<UpdateMemberAuthParams>();
            var result = fixture.Create<SingleSignOnResult>();
            memberAuthService.UpdateMemberAuth(Arg.Any<UpdateMemberAuthParams>()).Returns(result);

            // Act
            var actualValue = await controller.UpdateMemberAuth(updateParams);

            // Assert
            Assert.Equal(result, actualValue);
        }
    }
}