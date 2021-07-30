using System;
using System.Linq;
using System.Threading.Tasks;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators.Log;
using L1.Features.Sites;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignInResult = L1.WebApi.Models.SignInResult;

namespace L1.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class SignInController : ControllerBase
    {
        private readonly IMemberAuthService memberAuthService;
        private readonly ISiteDataService siteDataService;
        private readonly ILogService logService;

        public SignInController(IMemberAuthService memberAuthService, ISiteDataService siteDataService, ILogService logService)
        {
            this.memberAuthService = memberAuthService;
            this.siteDataService = siteDataService;
            this.logService = logService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("index")]
        public async Task<IActionResult> Index(SignIn model)
        {
            var requestTime = DateTimeOffset.UtcNow;

            if (await siteDataService.VerifyClient(model.Auth?.ClientId, model.Auth?.ClientSecret, model.Currency))
            {
                var ip = model.IP.Split(',').FirstOrDefault()?.Trim();

                var result = memberAuthService.SignIn(new MemberAuth
                {
                    Domain = model.Domain,
                    BrowserUserAgent = model.BrowserUserAgent,
                    Currency = model.Currency,
                    IP = ip,
                    MemberId = model.MemberId,
                    MemberName = model.MemberName,
                    Seq = model.Seq,
                    Language = model.Language,
                    SiteId = model.SiteId,
                    ClientId = model.Auth?.ClientId,
                    IsMobi = model.IsMobi,
                    SiteName = model.SiteName,
                    IsCash = model.IsCash
                }, model.GameTypeId);

                await WriteSignInLog(requestTime, model, result);

                return Ok(new SignInResult { RedirectUrl = result.RedirectUrl });
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