using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Client;
using Microsoft.AspNetCore.Mvc;
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

        public IEnumerable<Language> Languages => new List<Language>
        {
            new Language("en-US", "English"),
            new Language("zh-CN", "Chinese Simplified"),
            new Language("zh-TW", "Chinese Traditional")
        };

        public int MemberId { get; set; } = 1111111;

        public string MemberName { get; set; } = "MemberName";

        public string Language { get; set; } = "en-US";

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

        public async Task<IActionResult> OnPost()
        {
            MemberId = int.Parse(Request.Form[nameof(MemberId)]);
            MemberName = Request.Form[nameof(MemberName)];
            Language = Request.Form[nameof(Language)];

            var redirectUrl = await GenerateRedirecUrl();

            return Redirect(redirectUrl);
        }

        private async Task<string> GenerateRedirecUrl()
        {
            var authenticationInfo = new AuthenticationInfo(
                            apiUri: new Uri("http://api.lumigame.com/"),
                            authUri: new Uri("http://auth.lumigame.com/"),
                            clientId: "licensee1",
                            clientSecret: "123licensee1%^&sit",
                            siteId: "licensee1");

            var client = new AuthenticationClient(authenticationInfo);

            var signInParams = new SingleSignOnParams(
                siteId: authenticationInfo.SiteId,
                memberId: MemberId,
                memberName: MemberName,
                language: Language,
                iP: Request.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                browserUserAgent: Request.Headers["User-Agent"].ToString());

            SingleSignOnResult = await client.SingleSignOnAsync(signInParams);

            return SingleSignOnResult.RedirectUri.ToString();
        }
    }

    public class Language
    {
        public Language(string value, string name)
        {
            Value = value;
            Name = name;
        }

        public string Value { get; }

        public string Name { get; }
    }
}