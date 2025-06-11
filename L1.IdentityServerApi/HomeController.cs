using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace L1.IdentityServerApi
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Index() => Content("Auth is running!!!", "text/html");
    }
}