using System.Net;
using L1.Features.PartnerCommunicators;
using Microsoft.Net.Http.Headers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace L1.WebApi.IntegrationTests
{
    public class WireMockFixture
    {
        public WireMockServer Server { get; }

        public WireMockFixture()
        {
            Server = WireMockServer.Start(5001);

            Server.Given(Request.Create().WithPath("/*").UsingAnyMethod())
                        .AtPriority(1000)
                        .RespondWith(
                            Response.Create()
                            .WithStatusCode(HttpStatusCode.NotImplemented)
                            .WithHeader(HeaderNames.ContentType, "application/text")
                            .WithBody("Request not mapped!")
                        );
        }

        public void MockApiWithResponse<T>(string path, PartnerResponse<T> response) where T : PartnerResult
        {
            Server.Given(
                    Request.Create()
                        .WithPath("/OW/SabaClub/GetBalance")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(response)
                );
        }
    }
}