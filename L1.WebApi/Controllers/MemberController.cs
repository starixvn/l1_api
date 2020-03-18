namespace L1.WebApi.Controllers
{
    using System.Threading.Tasks;
    using L1.Features.OWCommunicators;
    using L1.WebApi.Models;
    using L1.WebApi.OW;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController, Route("[controller]"), Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IOWService oWService;

        public MemberController(IOWService oWService)
        {
            this.oWService = oWService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NotifyMemberLogin([FromBody] MemberLogin model)
        {
            if (ModelState.IsValid)
            {
                //TODO: call OW Api to notify member login
                var response = await oWService.Post<OWResult>("/notify-customer-login",
                    new CustomerLoginCall(model.MemberId, model.Seq, model.CharacterName, model.FirstLogin));

                if (response.Result.ErrorCode == 0)
                {
                    return Ok();
                }
            }

            return BadRequest(ModelState);
        }
    }
}