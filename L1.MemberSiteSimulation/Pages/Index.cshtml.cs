using System;
using System.Threading.Tasks;
using L1.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace L1.MemberSiteSimulation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public SingleSignOnResult SingleSignOnResult { get; private set; }

        public int MemberId { get; set; } = 1111111;

        public string MemberName { get; set; } = "MemberName";

        public async Task OnGet()
        {
            //var redirectUrl = await GenerateRedirecUrl();

            //if (SingleSignOnResult.IsSuccessful())
            //{
            //    // return singleSignOnResult.RedirectUri;
            //}
            //else
            //{
            //    // Handle Error
            //}
        }

        public async Task OnPost()
        {
            MemberId = int.Parse(Request.Form[nameof(MemberId)]);
            MemberName = Request.Form[nameof(MemberName)];

            var redirectUrl = await GenerateRedirecUrl();
        }

        private async Task<string> GenerateRedirecUrl()
        {
            var authenticationInfo = new AuthenticationInfo(
                            apiUri: new Uri("http://localhost:8868/"),
                            authUri: new Uri("http://localhost:8898/"),
                            clientId: "licensee1",
                            clientSecret: "123licensee1%^&sit",
                            siteId: "licensee1");

            var client = new AuthenticationClient(authenticationInfo);

            var signInParams = new SingleSignOnParams(
                siteId: authenticationInfo.SiteId,
                memberId: MemberId,
                memberName: MemberName,
                iP: Request.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                browserUserAgent: Request.Headers["User-Agent"].ToString());

            SingleSignOnResult = await client.SingleSignOnAsync(signInParams);

            return SingleSignOnResult.RedirectUri.ToString();
        }
    }
}