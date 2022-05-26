using System.Diagnostics;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Features.Partners;
using L1.Features.SGApiCommunicators;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace L1.WebApi.Controllers
{
    [ApiController, Authorize]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [Route("member")]
    public class PlayerController : ControllerBase
    {
        private readonly IPartnerCustomerFactory partnerCustomerFactory;
        private readonly ISGApiService sgApiService;
        private readonly IPartnerDataService partnerDataService;
        private readonly IPartnerRequestAuthentication partnerRequestAuthentication;

        public PlayerController(IPartnerCustomerFactory partnerCustomerFactory, 
            ISGApiService sgApiService,
            IPartnerDataService partnerDataService,
            IPartnerRequestAuthentication partnerRequestAuthentication)
        {
            this.partnerCustomerFactory = partnerCustomerFactory;
            this.sgApiService = sgApiService;
            this.partnerRequestAuthentication = partnerRequestAuthentication;
            this.partnerDataService = partnerDataService;
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

            Activity.Current?.AddTag("PlayerId", model.PlayerId);

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
                model.OperatorToken);

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
        [Route("/Integrity/[action]")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "DailySummarize")]
        public async Task<IActionResult> DailySummarize([FromBody] DailySummary model)
        {
            if (await VerifyRequest(model.Auth?.OperatorId))
            {
                var response = await sgApiService.DailySummary(new DailySummaryRequest 
                {
                    Auth = new () { OperatorId = model.Auth.OperatorId },
                    Seq = model.Seq,
                    SummarizedDate = model.SummarizedDate
                });

                return Ok(response);
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "RetrieveEndRoundInfo")]
        public async Task<IActionResult> RetrieveEndRoundInfo([FromBody] RetrieveEndRoundInfo model)
        {
            if (await VerifyRequest(model.Auth?.OperatorId))
            {
                var response = await sgApiService.RetrieveEndRoundInfoAsync(new RetrieveEndRoundInfoRequest 
                {
                    Auth = new () { OperatorId = model.Auth.OperatorId },
                    Seq = model.Seq,
                    GameRoundId = model.GameRoundId,
                    GameTypeId = model.GameTypeId,
                    PlayerId = model.PlayerId,
                    SiteId = model.SiteId
                });
                return Ok(response);
            }

            return Unauthorized();
        }

        private async Task<bool> VerifyRequest(string operatorId)
        {
            var @operator = await partnerDataService.GetById(operatorId);

            return await partnerRequestAuthentication.Verify(@operator?.SecretKey, Request);
        }
    }
}