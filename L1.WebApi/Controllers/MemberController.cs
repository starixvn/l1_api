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
        public IActionResult NotifyMemberLogin([FromBody] MemberLogin model)
        {
            if (ModelState.IsValid)
            {
                //TODO: call OW Api to notify member login
                return Ok();
            }

            return BadRequest(ModelState);
        }
    }
}