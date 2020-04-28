using L1.Features.MemberAuthentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController, Route("[controller]"), AllowAnonymous]
    public class AuthVerificationController
    {
        private readonly IMemberAuthService memberAuthService;

        public AuthVerificationController(IMemberAuthService memberAuthService)
        {
            this.memberAuthService = memberAuthService;
        }

        [HttpPost, Route("verify")]
        public VerificationResult Verify(VerificationParams verificationParams)
        {
            var result = memberAuthService.VerifyToken(verificationParams);

            return result;
        }
    }
}