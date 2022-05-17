using System;
using System.Linq;
using System.Threading.Tasks;
using L1.Features.MemberAuthentications;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.Partners;
using L1.Shared.Constants;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SignInResult = L1.WebApi.Models.SignInResult;

namespace L1.WebApi.Controllers
{
    [ApiController, Authorize]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class SignInController : ControllerBase
    {
        private readonly IMemberAuthService memberAuthService;
        private readonly IPartnerDataService partnerService;
        private readonly ILogService logService;
        private readonly IPartnerRequestAuthentication partnerRequestAuthentication;

        public SignInController(IMemberAuthService memberAuthService, IPartnerDataService partnerService, ILogService logService, IPartnerRequestAuthentication partnerRequestAuthentication)
        {
            this.memberAuthService = memberAuthService;
            this.partnerService = partnerService;
            this.logService = logService;
            this.partnerRequestAuthentication = partnerRequestAuthentication;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("index")]
        [SwaggerOperation(Summary = "SignIn")]
        public async Task<IActionResult> Index(SignIn model)
        {
            var requestTime = DateTimeOffset.UtcNow;

            if (await partnerService.VerifyWithCurrency(model.Auth?.OperatorId, model.Currency))
            {
                var partner = await partnerService.GetById(model.Auth.OperatorId);

                if (await partnerRequestAuthentication.Verify(partner?.SecretKey, Request))
                {
                    var ip = model.IP.Split(',').FirstOrDefault()?.Trim();

                    var result = await memberAuthService.SignIn(new MemberAuth
                    {
                        Domain = model.Domain,
                        BrowserUserAgent = model.BrowserUserAgent,
                        CurrencyName = model.Currency,
                        IP = ip,
                        PlayerId = model.PlayerId,
                        PlayerName = model.PlayerName,
                        Seq = model.Seq,
                        Language = model.Language,
                        SiteId = model.SiteId,
                        OperatorId = model.Auth.OperatorId,
                        AppType = AppTypes.IsValid(model.AppType) ? model.AppType : AppTypes.SabaClub,
                        SiteName = model.SiteName,
                        OperatorToken = model.OperatorToken
                    }, model.GameTypeId);

                    await WriteSignInLog(requestTime, model, result);

                    return Ok(new SignInResult { RedirectUrl = result.RedirectUrl });
                }
            }

            await WriteSignInLog(requestTime, model, SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.Unauthorize));

            return Unauthorized();
        }

        private Task WriteSignInLog(DateTimeOffset requestTime, SignIn model, SingleSignOnResult result)
        {
            var formattedResult = new SingleSignOnResult(result.Code, result.Message)
            {
                RedirectUrl = result.RedirectUrl
            };

            var log = new SBLog(model, formattedResult, requestTime, DateTimeOffset.UtcNow);

            return logService.LogSignInAsync(log);
        }
    }
}