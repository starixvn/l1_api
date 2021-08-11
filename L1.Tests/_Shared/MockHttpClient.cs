using System;
using System.Net;
using System.Net.Http;
using NSubstitute;

namespace L1.Tests._Shared
{
    public static class MockHttpClient
    {
        public static HttpClient Create(HttpStatusCode statusCode, string content)
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var httpClient = new HttpClient(new MockHttpMessageHandler(statusCode, content))
            {
                BaseAddress = new Uri("https://localhost")
            };

            httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            return httpClient;
        }
    }
}