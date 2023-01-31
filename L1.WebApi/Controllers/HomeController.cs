using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        public ActionResult Index() => new ContentResult
        {
            Content = "Api is running!!!",
            ContentType = "text/html"
        };
    }
}