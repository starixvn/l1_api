using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.WebApi.Models;
using Newtonsoft.Json;
using Xunit;

namespace L1.WebApi.IntegrationTests.Controllers
{
    public class SignInIntegrationTests : IClassFixture<AppTestFixture>
    {
        private const string defaultMediaType = "application/json";
        private readonly HttpClient client;

        public SignInIntegrationTests(AppTestFixture fixture)
        {
            client = fixture.CreateClient();
        }

        [Theory]
        [InlineData("/SignIn/Index")]
        public async Task Verify_Unauthorize_ReturnUnauthorize(string url)
        {
            // Arrange
            var signIn = new SignIn();
            var requestBody = JsonConvert.SerializeObject(signIn);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}