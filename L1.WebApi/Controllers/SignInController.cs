using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignInController : ControllerBase
    {
        [HttpGet]
        [Route("test")]
        public string Test() => "hello";
    }
}