using System.Threading.Tasks;
using L1.Features.MemberAuthentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController, Authorize]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthVerificationController
    {
        private readonly IMemberAuthService memberAuthService;

        public AuthVerificationController(IMemberAuthService memberAuthService)
        {
            this.memberAuthService = memberAuthService;
        }

        [HttpPost, Route("verify")]
        public Task<VerificationResult> Verify(VerificationParams verificationParams)
        => memberAuthService.VerifyToken(verificationParams);

        [HttpPost, Route("[action]")]
        public Task<SingleSignOnResult> UpdateMemberAuth(UpdateMemberAuthParams updateParams)
        => memberAuthService.UpdateMemberAuth(updateParams);
    }
}