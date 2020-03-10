using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignInController : ControllerBase
    {
        [HttpGet, HttpPost]
        [Route("test")]
        public string Test() => "http://demo.com";
    }
}