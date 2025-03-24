using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace L1.WebApi.Controllers
{
    [ApiController]
    [Route("")] 
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Index() => Content("Api is running!!!", "text/html");
    }
}