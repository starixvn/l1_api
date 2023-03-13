using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.PFFCommunicators;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.PFFCommunicators
{
    public class PFFServiceTests
    {
        private readonly IPFFFailureHandler failureHandler;

        private readonly object defaultContent = new
        {
            Result = new PartnerResult()
        };

        public PFFServiceTests()
        {
            failureHandler = Substitute.For<IPFFFailureHandler>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(PFFCall call, PFFServiceSettings oWServiceSettings)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(defaultContent));
            var service = new PFFService(httpClient, failureHandler, oWServiceSettings);

            // Act
            var result = await service.Post<PartnerResult>("/aaa", call);

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(PFFCall call, PFFServiceSettings oWServiceSettings)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, JsonConvert.SerializeObject(string.Empty));
            var service = new PFFService(httpClient, failureHandler, oWServiceSettings);

            // Act
            var response = await service.Post<PartnerResult>("/aaa", call);

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(response.Result.ErrorCode, PartnerSpecialErrorCodes.NetworkError);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}