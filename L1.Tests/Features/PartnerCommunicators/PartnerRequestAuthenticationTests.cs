using System.IO;
using System.Text;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.PartnerCommunicators
{
    public class PartnerRequestAuthenticationTests
    {
        private readonly IPartnerRequestAuthentication service;
        private readonly HttpRequest httpRequest;

        public PartnerRequestAuthenticationTests()
        {
            service = new PartnerRequestAuthentication();
            httpRequest = Substitute.For<HttpRequest>();

            httpRequest.Body = new MemoryStream(Encoding.ASCII.GetBytes("data"));
        }

        [Fact]
        public async Task Verify_ValidInputData_ReturnTrue()
        {
            httpRequest.Headers["Access-Key"].Returns((StringValues)"GywWt1vSqHDBFBU8zaW8/KYzFLxyL6Fg1pDeEzzLuds=");
            // Act
            var result = await service.Verify("secret", httpRequest);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "access-key")]
        [InlineData("secret", "access-key")]
        public async Task Verify_InvalidSecretKeyOrAccessKey_ReturnFalse(string secretKey, StringValues accessKey)
        {
            httpRequest.Headers["Access-Key"].Returns(accessKey);
            // Act
            var result = await service.Verify(secretKey, httpRequest);

            //Assert
            Assert.False(result);
        }
    }
}