namespace L1.WebApi.Controllers
{
    using L1.Features.Authentications;
    using L1.WebApi.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController, Route("[controller]"), Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IMemberAuthService memberAuthService;

        public MemberController(IMemberAuthService memberAuthService)
        {
            this.memberAuthService = memberAuthService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("character/update")]
        public IActionResult UpdateCharacterName([FromBody] UpdateCharacterName model)
        {
            return NoContent();
        }
    }
}