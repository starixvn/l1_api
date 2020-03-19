namespace L1.WebApi.Controllers
{
    using System.Threading.Tasks;
    using L1.Features.OWCommunicators;
    using L1.WebApi.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController, Route("[controller]"), Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IOWCustomerService oWCustomerService;

        public MemberController(IOWCustomerService oWCustomerService)
        {
            this.oWCustomerService = oWCustomerService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NotifyMemberLogin([FromBody] MemberLogin model)
        {

            var response = await oWCustomerService.NotifyLogin(
                new NotifyCustomerLoginCall(model.MemberId, model.Seq, model.SiteId, model.CharacterName, model.FirstLogin));

            return Ok(response);
        }
    }
}