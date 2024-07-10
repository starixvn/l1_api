using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Award;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.HitJackpot;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace L1.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [Route("member")]
    public class PlayerController : ControllerBase
    {
        private readonly IPartnerCustomerFactory partnerCustomerFactory;

        public PlayerController(IPartnerCustomerFactory partnerCustomerFactory)
        {
            this.partnerCustomerFactory = partnerCustomerFactory;
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "EnterPortal")]
        public async Task<IActionResult> EnterPortal([FromBody] MemberEnterPortal model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.EnterPortal(
                new PartnerEnterPortalCall(model.PlayerId, model.SiteId, model.CharacterName, model.FirstLogin, model.OperatorToken));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "GetBalance")]
        public async Task<IActionResult> Balance([FromBody] GetBalance model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.GetBalance(
                new PartnerGetBalanceCall(model.PlayerId, model.SiteId, model.OperatorToken));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "PlaceBet")]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBet model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var placeBetCall = new PartnerPlaceBetCall(
                model.PlayerId,
                model.SiteId,
                model.GameRoundId,
                model.GameTypeId,
                model.ChoiceId,
                model.Amount,
                model.Currency,
                model.RoundEndTime,
                model.Ip,
                model.FreeSpin,
                model.ContributionAmount,
                model.ContributionRate,
                model.FreeBet,
                model.OperatorToken,
                model.OperatorId,
                model.UserId);

            var response = await customerService.PlaceBet(placeBetCall, model.System, model.Metadata);

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "EndGame")]
        public async Task<IActionResult> EndGame([FromBody] EndGame model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.EndGame(
                new PartnerEndGameCall(
                    model.PlayerId,
                    model.SiteId,
                    model.GameRoundId,
                    model.GameTypeId,
                    model.TotalAmount,
                    model.TotalWin,
                    model.ValidBetAmount,
                    model.EndTime,
                    model.FreeBet));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "VoidGame")]
        public async Task<IActionResult> VoidGame([FromBody] VoidGame model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.VoidGame(
                new PartnerVoidGameCall(model.PlayerId, model.SiteId, model.GameRoundId, model.GameTypeId, model.Reason, model.FreeBet));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "GetBetLimit")]
        public async Task<IActionResult> GetBetLimit([FromBody] GetBetLimit model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.GetBetLimit(new PartnerGetBetLimitCall(model.PlayerId, model.SiteId, model.OperatorToken));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "HitJackpot")]
        public async Task<IActionResult> HitJackpot([FromBody] HitJackpot model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.HitJackpot(new HitJackpotCall(
                model.PlayerId,
                model.SiteId,
                model.GameRoundId,
                model.GameTypeId,
                model.WinningAmount));

            return Ok(response);
        }


        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "Award")]
        public async Task<IActionResult> Award([FromBody] Award model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.Award(
                new PartnerAwardCall(
                    model.PlayerId,
                    model.GameTypeId,
                    model.Amount,
                    model.PromotionId));

            return Ok(response);
        }
    }
}