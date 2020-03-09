using Microsoft.AspNetCore.Mvc;

namespace L1.WebApi.Controllers
{
    public class HomeController : Controller
    {
        [Route("/home")]
        public ActionResult Index() => new ContentResult()
        {
            Content = "Site is running",
            ContentType = "text/html"
        };
    }
}