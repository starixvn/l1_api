using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.WebApi.Models;
using Newtonsoft.Json;
using Xunit;

namespace L1.WebApi.IntegrationTests.Controllers
{
    public class GetBalanceIntegrationTests : IClassFixture<AppTestFixture>
    {
        private const string defaultMediaType = "application/json";
        private readonly HttpClient client;
        private readonly WireMockFixture wireMockFixture;

        public GetBalanceIntegrationTests(AppTestFixture fixture)
        {
            client = fixture.CreateClient();
            wireMockFixture = new WireMockFixture();
        }

        [Theory]
        [InlineData("/Member/Balance")]
        public async Task GetBalance_Unauthorize_ReturnUnauthorize(string url)
        {
            // Arrange
            var request = new GetBalance { PartnerId = "ONEworks1", ObCustId = 123, SiteId = "1414" };
            var requestBody = JsonConvert.SerializeObject(request);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("/Member/Balance")]
        public async Task GetBalance_Authorize_ReturnBalance(string url)
        {
            // Arrange
            wireMockFixture.MockApiWithResponse(
                "",
                new PartnerResponse<PartnerGetBalanceResult>
                {
                    Result = new PartnerGetBalanceResult { Balance = 11111 }
                });

            var request = new GetBalance { PartnerId = "ONEworks1", ObCustId = 123, SiteId = "1414" };
            var requestBody = JsonConvert.SerializeObject(request);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);
            var token = await GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private static Task<string> GetToken()
        {
            var auth = new IdentityServerAuth
            {
                ClientId = "admin-api",
                ClientSecret = "adminapi@20luminet&",
                Url = "http://l1-auth1.lumigame.com/",
                SiteId = "admin-api"
            };
            IdentityServerManager.Initialize(auth);

            return IdentityServerManager.GetAccessToken();
        }
    }
}