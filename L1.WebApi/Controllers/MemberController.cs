using System;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.DailySummary;
using L1.Features.AdminApiCommunicators.RetrieveEndGameInfo;
using L1.Features.Partners;
using L1.Features.Partners.EndGame;
using L1.Features.Partners.EnterPortal;
using L1.Features.Partners.GetBalance;
using L1.Features.Partners.GetBetLimit;
using L1.Features.Partners.Log;
using L1.Features.Partners.PlaceBet;
using L1.Features.Partners.VoidGame;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController, Authorize]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IAdminApiService adminApiService;
        private readonly IPartnerDataService partnerDataService;
        private readonly ILogService logService;
        private readonly IPartnerCustomerFactory partnerCustomerFactory;

        public MemberController(
            IAdminApiService adminApiService,
            IPartnerDataService partnerDataService,
            ILogService logService,
            IPartnerCustomerFactory partnerCustomerFactory)
        {
            this.adminApiService = adminApiService;
            this.partnerDataService = partnerDataService;
            this.logService = logService;
            this.partnerCustomerFactory = partnerCustomerFactory;
        }

        [HttpPost]
        [Route("[action]")]
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
                model.FreeSpin);

            var response = await customerService.PlaceBet(placeBetCall, model.System);

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
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
                    model.EndTime));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> VoidGame([FromBody] VoidGame model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.PartnerId);

            if (customerService == default)
            {
                return BadRequest();
            }

            var response = await customerService.VoidGame(
                new PartnerVoidGameCall(model.ObCustId, model.SiteId, model.GameRoundId, model.GameTypeId, model.Reason));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
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
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveEndGameInfo([FromBody] EndGameInfoRequest model)
        {
            if (siteDataService.VerifyClient(model.Auth?.PartnerId, model.Auth?.PartnerSecret))
            {
                var requestTime = DateTimeOffset.UtcNow;
                var response = await adminApiService.RetrieveEndGameInfoAsync(
                    new RetrieveEndGameInfoRequest(
                        model.GameRoundId,
                        model.GameTypeId,
                        model.ObCustId,
                        model.SiteId,
                        model.Auth.PartnerId));

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
        public async Task<IActionResult> DailySummarize([FromBody] DailySummary model)
        {
            if (siteDataService.VerifyClient(model.Auth?.PartnerId, model.Auth?.PartnerSecret))
            {
                var response = await adminApiService.DailySummary(new DailySummaryRequest(model.Auth.PartnerId, model.SummarizedDate));
                response.Result.Seq = model.Seq;

                return Ok(response);
            }

            return Unauthorized();
        }
    }
}