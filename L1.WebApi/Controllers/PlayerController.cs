using System;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Award;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.HitJackpot;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.Promotion;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
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
        private readonly IPartnerPlayerIdConverter partnerPlayerIdConverter;
        private readonly ISentryClient sentryClient;

        public PlayerController(IPartnerCustomerFactory partnerCustomerFactory, IPartnerPlayerIdConverter partnerPlayerIdConverter, ISentryClient sentryClient)
        {
            this.partnerCustomerFactory = partnerCustomerFactory;
            this.partnerPlayerIdConverter = partnerPlayerIdConverter;
            this.sentryClient = sentryClient;
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "EnterPortal")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartnerResponse<PartnerEnterPortalResult>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PartnerResponse<PartnerEnterPortalResult>>> EnterPortal([FromBody] MemberEnterPortal model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest(new { ErrorCode = 1004, ErrorDescription = "Partner ID is invalid" });
            }

            var response = await customerService.EnterPortal(
                new PartnerEnterPortalCall(partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId), model.SiteId, model.CharacterName, model.FirstLogin, model.OperatorToken));

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

            try
            {

                var response = await customerService.GetBalance(
                new PartnerGetBalanceCall(partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId), model.SiteId, model.OperatorToken));

                return Ok(response);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return BadRequest();
            }
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

            try
            {
                var placeBetCall = new PartnerPlaceBetCall(
                partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId),
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
                model.UserId,
                model.PromotionId,
                model.PromotionCode);

                var response = await customerService.PlaceBet(placeBetCall, model.System, model.Metadata);
                return Ok(response);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return BadRequest();
            }
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
                    partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId),
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
                new PartnerVoidGameCall(partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId), model.SiteId, model.GameRoundId, model.GameTypeId, model.Reason, model.FreeBet));

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

            var response = await customerService.GetBetLimit(new PartnerGetBetLimitCall(partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId), model.SiteId, model.OperatorToken));

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
                partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId),
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
                    partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId),
                    model.SiteId,
                    model.GameTypeId,
                    model.Amount,
                    model.PromotionId,
                    model.Currency));

            return Ok(response);
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = "StartPromotion")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartnerResponse<PartnerStartPromotionResult>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartPromotion([FromBody] StartPromotion model)
        {
            var customerService = partnerCustomerFactory.GetCustomerService(model.OperatorId);

            if (customerService == default)
            {
                return BadRequest("Invalid operator ID");
            }

            var validationResult = ValidateStartPromotionRequest(model);
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                DateTime startDate = DateTime.Parse(model.Start).ToUniversalTime();
                DateTime endDate = DateTime.Parse(model.End).ToUniversalTime();

                var response = await customerService.StartPromotion(
                    new PartnerStartPromotionCall(
                        partnerPlayerIdConverter.ConvertPlayerId(model.OperatorId, model.PlayerId).ToString(),
                        model.SiteId,
                        model.PromotionCode,
                        startDate,
                        endDate));

                return Ok(response);
            }
            catch (FormatException ex)
            {
                sentryClient.CaptureException(ex);
                return BadRequest(new { ErrorCode = 1007, ErrorDescription = "Invalid date format. Please use format: yyyy/MM/dd HH:mm:ss.fff +00:00" });
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                return BadRequest(new { ErrorCode = 1007, ErrorDescription = "An error occurred while processing the request" });
            }
        }

        private IActionResult ValidateStartPromotionRequest(StartPromotion model)
        {
            if (string.IsNullOrWhiteSpace(model.PromotionCode))
            {
                return BadRequest(new { ErrorCode = 1007, ErrorDescription = "PromotionCode is required" });
            }

            if (string.IsNullOrWhiteSpace(model.Start) || string.IsNullOrWhiteSpace(model.End))
            {
                return BadRequest(new { ErrorCode = 1007, ErrorDescription = "Start and End dates are required" });
            }

            return null;
        }
    }
}