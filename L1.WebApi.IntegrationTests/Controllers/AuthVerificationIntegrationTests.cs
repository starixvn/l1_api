using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.MemberAuthentications;
using Newtonsoft.Json;
using Xunit;

namespace L1.WebApi.IntegrationTests.Controllers
{
    public class AuthVerificationIntegrationTests : IClassFixture<AppTestFixture>
    {
        private const string defaultMediaType = "application/json";
        private readonly AppTestFixture fixture;
        private readonly HttpClient client;

        public AuthVerificationIntegrationTests(AppTestFixture fixture)
        {
            this.fixture = fixture;

            client = fixture.CreateClient();
        }

        [Theory]
        [InlineData("/AuthVerification/verify")]
        public async Task Verify_InvalidToken_ReturnTokenNotFoundCode(string url)
        {
            // Arrange
            var verificationParams = new VerificationParams();
            var requestBody = JsonConvert.SerializeObject(verificationParams);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}