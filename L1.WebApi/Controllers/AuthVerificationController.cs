using System.Threading.Tasks;
using L1.Features.MemberAuthentications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthVerificationController : ControllerBase
    {
        private readonly IMemberAuthService memberAuthService;

        public AuthVerificationController(IMemberAuthService memberAuthService)
        {
            this.memberAuthService = memberAuthService;
        }

        [HttpPost("verify")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerificationResult))]
        public async Task<ActionResult<VerificationResult>> Verify([FromBody] VerificationParams verificationParams)
        {
            var result = await memberAuthService.VerifyToken(verificationParams);
            return Ok(result);
        }
    }
}