using System.Net;
using System.Net.Http;
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
        public async Task Verify_Unauthorize_ReturnUnauthorize(string url)
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

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}