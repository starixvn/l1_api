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
        private readonly HttpClient client;

        public AuthVerificationIntegrationTests(AppTestFixture fixture)
        {
            client = fixture.CreateClient();
        }

        [Theory]
        [InlineData("/AuthVerification/verify")]
        public async Task Verify_Unauthorize_ReturnUnauthorize(string url)
        {
            // Arrange
            var verificationParams = new VerificationParams();
            var requestBody = JsonConvert.SerializeObject(verificationParams);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}