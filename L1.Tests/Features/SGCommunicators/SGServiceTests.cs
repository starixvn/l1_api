using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.PartnerCommunicators;
using L1.Features.SGCommunicators;
using L1.Shared.Json;
using L1.Tests._Shared;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.SGCommunicators
{
    public class SGServiceTests
    {
        private readonly ISGFailureHandler failureHandler;
        private readonly IJsonService jsonService;
        private readonly SGServiceSettings settings;
        private readonly PartnerResponse<PartnerResult> defaultContent;
        private const int TestPlayerId = 12345;

        public SGServiceTests()
        {
            failureHandler = Substitute.For<ISGFailureHandler>();
            jsonService = Substitute.For<IJsonService>();
            settings = new SGServiceSettings
            {
                SecretKey = "test-secret",
                BaseUrl = new Uri("https://localhost")
            };
            defaultContent = new PartnerResponse<PartnerResult>
            {
                Result = new PartnerResult { Seq = "123", ErrorCode = 0 }
            };
        }

        [Theory, AutoData]
        public async Task Post_ShouldAddProperAccessKeyHeader(SGCommunicatorCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var endpoint = "test-endpoint";
            var serializedContent = jsonService.Serialize(defaultContent);
            jsonService.Serialize(Arg.Any<PartnerRequest>()).Returns(serializedContent);
            jsonService.Deserialize<PartnerResponse<PartnerResult>>(serializedContent)
                .Returns(defaultContent);

            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, serializedContent);
            var service = new SGService(httpClient, failureHandler, settings, jsonService);

            // Act
            var result = await service.Post<PartnerResult>(endpoint, call, "SG");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(result);
            Assert.Equal(0, result.Result.ErrorCode);
            Assert.Equal("123", result.Result.Seq);
        }

        [Theory, AutoData]
        public async Task Post_NetworkError_ShouldReturnProperErrorResponse(SGCommunicatorCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var endpoint = "test-endpoint";
            var serializedContent = jsonService.Serialize(string.Empty);
            jsonService.Serialize(Arg.Any<PartnerRequest>()).Returns(serializedContent);

            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, serializedContent);
            var service = new SGService(httpClient, failureHandler, settings, jsonService);

            // Act
            var response = await service.Post<PartnerResult>(endpoint, call, "SG");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(PartnerSpecialErrorCodes.NetworkError, response.Result.ErrorCode);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }

        [Theory, AutoData]
        public async Task Post_HttpRequestException_ShouldReturnProperErrorResponse(SGCommunicatorCall call)
        {
            // Arrange
            call.PlayerId = TestPlayerId;
            var endpoint = "test-endpoint";
            var serializedContent = jsonService.Serialize(string.Empty);
            jsonService.Serialize(Arg.Any<PartnerRequest>()).Returns(serializedContent);

            var httpClient = MockHttpClient.CreateErrorClient();
            var service = new SGService(httpClient, failureHandler, settings, jsonService);

            // Act
            var response = await service.Post<PartnerResult>(endpoint, call, "SG");

            // Assert
            Assert.IsType<PartnerResponse<PartnerResult>>(response);
            Assert.Equal(PartnerSpecialErrorCodes.NetworkError, response.Result.ErrorCode);
            Assert.Contains("Network error occurred", response.Result.ErrorDescription);
            Assert.Equal(call.Seq, response.Result.Seq);
        }
    }
}