using L1.Features.Authentications;
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
        public string Index(MemberAuth memberAuth)
            => memberAuthService.SignIn(memberAuth).RedirectUri.ToString();
    }
}