using L1.Features.Authentications;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class SignInController : ControllerBase
    {
        private readonly IMemberAuthService memberAuthService;

        public SignInController(IMemberAuthService memberAuthService)
        {
            this.memberAuthService = memberAuthService;
        }

        [AllowAnonymous]
        [HttpGet, HttpPost]
        [Route("index")]
        public string Index(SignIn model)
        {
            return memberAuthService.SignIn(new MemberAuth
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
                ClientSecret = model.Auth?.ClientSecret,
            }).RedirectUri.ToString();
        }
    }
}