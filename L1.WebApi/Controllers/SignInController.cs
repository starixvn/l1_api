using L1.Features.Authentications;
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

        public SignInController(IMemberAuthService memberAuthService, ISiteDataService siteDataService)
        {
            this.memberAuthService = memberAuthService;
            this.siteDataService = siteDataService;
        }

        [AllowAnonymous]
        [HttpGet, HttpPost]
        [Route("index")]
        public IActionResult Index(SignIn model)
        {
            if (siteDataService.VerifyClient(model.Auth?.ClientId, model.Auth?.ClientSecret))
            {
                var redirectUrl = memberAuthService.SignIn(new MemberAuth
                {
                    BrowserUserAgent = model.BrowserUserAgent,
                    Currency = model.Currency,
                    IP = model.IP,
                    MemberId = model.MemberId,
                    MemberName = model.MemberName,
                    Seq = model.Seq,
                    Language = model.Language,
                    SiteId = model.SiteId,
                    ClientId = model.Auth?.ClientId,
                }, model.GameTypeId).RedirectUri.ToString();

                return Ok(new SignInResult { RedirectUrl = redirectUrl });
            }

            return Unauthorized();
        }
    }
}