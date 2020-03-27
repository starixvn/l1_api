using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L1.IdentityServerApi
{
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        public ActionResult Index() => new ContentResult()
        {
            Content = "Auth server is running!!!",
            ContentType = "text/html"
        };
    }
}