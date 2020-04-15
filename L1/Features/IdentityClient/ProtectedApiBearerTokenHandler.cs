using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace L1.Features.IdentityClient
{
    public class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IIdentityService identityService;

        public ProtectedApiBearerTokenHandler(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await identityService.GetAccessToken();

            request.SetBearerToken(accessToken.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}