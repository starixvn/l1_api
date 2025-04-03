using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace L1.Tests._Shared
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        public Uri RequestUri { get; private set; }
        public HttpContent RequestContent { get; private set; }
        public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;
        public string ResponseContent { get; set; } = "{}";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            RequestContent = request.Content;

            var response = new HttpResponseMessage
            {
                StatusCode = ResponseStatusCode,
                Content = new StringContent(ResponseContent)
            };
            response.Headers.Add("X-Requested-Uri", RequestUri.AbsolutePath.TrimStart('/'));

            return Task.FromResult(response);
        }
    }
}