using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    public class SignInController : ControllerBase
    {
        [HttpGet, HttpPost]
        [Route("test")]
        public string Test() => "http://demo.com";
    }
}