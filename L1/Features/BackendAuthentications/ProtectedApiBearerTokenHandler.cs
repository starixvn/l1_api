using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace L1.Features.BackendAuthentications
{
    public class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IBackendAuthService identityService;

        public ProtectedApiBearerTokenHandler(IBackendAuthService identityService)
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