using System;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.DailySummary;
using L1.Features.AdminApiCommunicators.RetrieveEndGameInfo;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.HitJackpot;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Features.Partners;
using L1.WebApi.Extensions;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace L1.WebApi.Controllers
{
    [ApiController, Authorize]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerIgnore]
    public class MemberController : ControllerBase
    {
        private readonly IAdminApiService adminApiService;
        private readonly IPartnerDataService partnerDataService;
        private readonly ILogService logService;
        private readonly IPartnerCustomerFactory partnerCustomerFactory;
        private readonly IPartnerRequestAuthentication partnerRequestAuthentication;

        public MemberController(
            IAdminApiService adminApiService,
            IPartnerDataService partnerDataService,
            ILogService logService,
            IPartnerCustomerFactory partnerCustomerFactory,
            IPartnerRequestAuthentication partnerRequestAuthentication)
        {
            this.adminApiService = adminApiService;
            this.partnerDataService = partnerDataService;
            this.logService = logService;
            this.partnerCustomerFactory = partnerCustomerFactory;
            this.partnerRequestAuthentication = partnerRequestAuthentication;
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "EnterPortal")]
        public async Task<IActionResult> EnterPortal([FromBody] MemberEnterPortal model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.EnterPortal(
                new PartnerEnterPortalCall(model.Seq, model.ObCustId, model.SiteId, model.CharacterName, model.FirstLogin));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "GetBalance")]
        public async Task<IActionResult> Balance([FromBody] GetBalance model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.GetBalance(
                new PartnerGetBalanceCall(model.ObCustId, model.SiteId));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "PlaceBet")]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBet model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var placeBetCall = new PartnerPlaceBetCall(
                model.ObCustId,
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
                model.FreeBet);

            var response = await customerService.PlaceBet(placeBetCall, model.System);

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "EndGame")]
        public async Task<IActionResult> EndGame([FromBody] EndGame model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.EndGame(
                new PartnerEndGameCall(
                    model.ObCustId,
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
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.VoidGame(
                new PartnerVoidGameCall(model.ObCustId, model.SiteId, model.GameRoundId, model.GameTypeId, model.Reason, model.FreeBet));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "GetBetLimit")]
        public async Task<IActionResult> GetBetLimit([FromBody] GetBetLimit model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.GetBetLimit(new PartnerGetBetLimitCall(model.ObCustId, model.SiteId));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "HitJackpot")]
        public async Task<IActionResult> HitJackpot([FromBody] HitJackpot model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.HitJackpot(new HitJackpotCall(
                model.ObCustId,
                model.SiteId,
                model.GameRoundId,
                model.GameTypeId,
                model.WinningAmount));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "RetrieveEndGame")]
        public async Task<IActionResult> RetrieveEndGameInfo([FromBody] EndGameInfoRequest model)
        {
            if (await VerifyRequest(model.Auth))
            {
                var requestTime = DateTimeOffset.UtcNow;
                var response = await adminApiService.RetrieveEndGameInfoAsync(
                    new RetrieveEndGameInfoRequest(
                        model.GameRoundId,
                        model.GameTypeId,
                        model.ObCustId,
                        model.SiteId,
                        model.Auth.OperatorId,
                        model.FreeBet));

                response.Result.Seq = model.Seq;

                await WriteRetrieveEndGameLog(requestTime, model, response.Result);

                return Ok(response);
            }

            return Unauthorized();
        }

        private Task WriteRetrieveEndGameLog(DateTimeOffset requestTime, EndGameInfoRequest request, RetrieveEndGameInfoResult result)
        {
            var log = new SBLog(request, result, requestTime, DateTimeOffset.UtcNow);

            return logService.LogRetrieveEndGameAsync(log);
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "DailySummarize")]
        public async Task<IActionResult> DailySummarize([FromBody] DailySummary model)
        {
            if (await VerifyRequest(model.Auth))
            {
                var response = await adminApiService.DailySummary(new DailySummaryRequest(model.Auth.OperatorId, model.SummarizedDate));
                response.Result.Seq = model.Seq;

                return Ok(response);
            }

            return Unauthorized();
        }

        private async Task<bool> VerifyRequest(Auth auth)
        {
            var @operator = await partnerDataService.GetById(auth?.OperatorId);

            return await partnerRequestAuthentication.Verify(@operator?.SecretKey, Request);
        }
    }
}