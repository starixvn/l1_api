using System.Threading.Tasks;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.NotifyLogin;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
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
        [Route("[action]")]
        public async Task<IActionResult> NotifyLogin([FromBody] MemberLogin model)
        {

            var response = await oWCustomerService.NotifyLogin(
                new NotifyLoginCall(model.MemberId, model.Seq, model.SiteId, model.CharacterName, model.FirstLogin));

            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> Balance([FromBody] GetBalance model)
        {

            var response = await oWCustomerService.GetBalance(
                new GetBalanceCall(model.Seq, model.MemberId, model.SiteId));

            return Ok(response);
        }
    }
}