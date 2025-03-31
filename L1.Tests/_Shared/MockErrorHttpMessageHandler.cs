using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace L1.Tests._Shared
{
    public class MockErrorHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new HttpRequestException("Network error occurred");
        }
    }
}