using System;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.DailySummary;
using L1.Features.AdminApiCommunicators.RetrieveEndGameInfo;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.HitJackpot;
using L1.Features.OWCommunicators.GetBetLimit;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
using L1.Features.Sites;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IOWCustomerService oWCustomerService;
        private readonly IAdminApiService adminApiService;
        private readonly ISiteDataService siteDataService;
        private readonly ILogService logService;

        public MemberController(
            IOWCustomerService oWCustomerService,
            IAdminApiService adminApiService,
            ISiteDataService siteDataService,
            ILogService logService)
        {
            this.oWCustomerService = oWCustomerService;
            this.adminApiService = adminApiService;
            this.siteDataService = siteDataService;
            this.logService = logService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EnterPortal([FromBody] MemberEnterPortal model)
        {
            var response = await oWCustomerService.EnterPortal(
                new EnterPortalCall(model.Seq, model.ObCustId, model.SiteId, model.CharacterName, model.FirstLogin));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Balance([FromBody] GetBalance model)
        {
            var response = await oWCustomerService.GetBalance(
                new GetBalanceCall(model.ObCustId, model.SiteId));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBet model)
        {
            var placeBetCall = new PlaceBetCall(
                model.ObCustId,
                model.SiteId,
                model.GameRoundId,
                model.GameTypeId,
                model.ChoiceId,
                model.Amount,
                model.Currency,
                model.RoundEndTime,
                model.Ip,
                model.FreeBet,
                model.FreeSpin,
                model.ContributionRate,
                model.ContributionAmount);
            var response = await oWCustomerService.PlaceBet(placeBetCall, model.System);

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EndGame([FromBody] EndGame model)
        {
            var response = await oWCustomerService.EndGame(
                new EndGameCall(
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
        public async Task<IActionResult> VoidGame([FromBody] VoidGame model)
        {
            var response = await oWCustomerService.VoidGame(
                new VoidGameCall(model.ObCustId, model.SiteId, model.GameRoundId, model.GameTypeId, model.Reason, model.FreeBet));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveEndGameInfo([FromBody] EndGameInfoRequest model)
        {
            if (siteDataService.VerifyClient(model.Auth?.ClientId, model.Auth?.ClientSecret))
            {
                var requestTime = DateTimeOffset.UtcNow;
                var response = await adminApiService.RetrieveEndGameInfoAsync(
                    new RetrieveEndGameInfoRequest(
                        model.GameRoundId,
                        model.GameTypeId,
                        model.ObCustId,
                        model.SiteId,
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
        public async Task<IActionResult> DailySummarize([FromBody] DailySummary model)
        {
            if (siteDataService.VerifyClient(model.Auth?.ClientId, model.Auth?.ClientSecret))
            {
                var response = await adminApiService.DailySummary(new DailySummaryRequest(model.SummarizedDate));
                response.Result.Seq = model.Seq;

                return Ok(response);
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> HitJackpot([FromBody] HitJackpot model)
        {
            var hitJackpotCall = new HitJackpotCall(
                model.ObCustId,
                model.SiteId,
                model.GameRoundId,
                model.GameTypeId,
                model.WinningAmount);
            var response = await oWCustomerService.HitJackpot(hitJackpotCall);

            return Ok(response);
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetBetLimit([FromBody] GetBetLimit model)
        {
            var response = await oWCustomerService.GetBetLimit(new GetBetLimitCall(model.ObCustId, model.SiteId));

            return Ok(response);
        }
    }
}