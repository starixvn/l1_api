using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.OWCommunicators;
using L1.Shared.Configurations;
using L1.Tests._Shared;
using Newtonsoft.Json;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.OWCommunicators
{
    public class OWServiceTests
    {
        private readonly IOWFailureHandler failureHandler;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;

        private readonly OWResponse<OWResult> defaultContent = new OWResponse<OWResult>
        {
            Result = new OWResult()
        };

        public OWServiceTests()
        {
            failureHandler = Substitute.For<IOWFailureHandler>();
            appSettings = Substitute.For<IAppSettings>();
            appSettings.OWServiceSettings = new OWServiceSettings { SecretKey = "aaa" };
            sentryClient = Substitute.For<ISentryClient>();
        }

        [Theory, AutoData]
        public async Task Post_Success_ReturnsOWResponse(OWCall call)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.OK, JsonConvert.SerializeObject(defaultContent));
            var service = new OWService(httpClient, failureHandler, appSettings, sentryClient);

            // Act
            var result = await service.Post<OWResult>("/aaa", call);

            // Assert
            Assert.IsType<OWResponse<OWResult>>(result);
        }

        [Theory, AutoData]
        public async Task Post_Failed_ReturnsOWResponse(OWCall call)
        {
            // Arrange
            var httpClient = MockHttpClient.Create(HttpStatusCode.BadGateway, JsonConvert.SerializeObject(string.Empty));
            var service = new OWService(httpClient, failureHandler, appSettings, sentryClient);

            // Act
            var response = await service.Post<OWResult>("/aaa", call);

            // Assert
            Assert.IsType<OWResponse<OWResult>>(response);
            Assert.Equal(response.Result.ErrorCode, OWSpecialErrorCodes.NetworkError);
            Assert.Contains(HttpStatusCode.BadGateway.ToString(), response.Result.ErrorDescription);
        }
    }
}