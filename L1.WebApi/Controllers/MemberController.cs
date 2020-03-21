using System.Threading.Tasks;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
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
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> EnterPortal([FromBody] MemberEnterPortal model)
        {
            var response = await oWCustomerService.EnterPortal(
                new EnterPortalCall(model.Seq, model.MemberId, model.SiteId, model.CharacterName, model.FirstLogin));

            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> Balance([FromBody] GetBalance model)
        {
            var response = await oWCustomerService.GetBalance(
                new GetBalanceCall(model.MemberId, model.SiteId));

            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBet model)
        {
            var response = await oWCustomerService.PlaceBet(
                new PlaceBetCall(model.MemberId, model.SiteId, model.GameRoundId, model.GameTypeId, model.ChoiceId, model.Amount, model.Currency, model.Ip));

            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> EndGame([FromBody] EndGame model)
        {
            var response = await oWCustomerService.EndGame(
                new EndGameCall(model.MemberId, model.SiteId, model.GameRoundId, model.GameTypeId, model.TotalAmount, model.TotalWin, model.ValidBetAmount));

            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> VoidGame([FromBody] VoidGame model)
        {
            var response = await oWCustomerService.VoidGame(
                new VoidGameCall(model.MemberId, model.SiteId, model.GameRoundId, model.GameTypeId, model.Reason));

            return Ok(response);
        }
    }
}