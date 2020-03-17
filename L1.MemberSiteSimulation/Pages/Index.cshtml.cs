using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Client;
using L1.MemberSiteSimulation.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace L1.MemberSiteSimulation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppSettings appSettings;

        public IndexModel(ILogger<IndexModel> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
            appSettings = options.Value;
        }

        public SingleSignOnResult SingleSignOnResult { get; private set; }

        public IEnumerable<Language> Languages => new List<Language>
        {
            new Language("en-US", "English"),
            new Language("zh-CN", "Chinese Simplified"),
            new Language("zh-TW", "Chinese Traditional")
        };

        [BindProperty]
        public int MemberId { get; set; } = 1111111;

        [BindProperty]
        public string MemberName { get; set; } = "MemberName";

        [BindProperty]
        public string Language { get; set; } = "en-US";

        [BindProperty]
        public string Currency { get; set; } = "UUS";

        public string ErrorMessage { get; set; }

        public string ClientId { get; set; } = "licensee1";

        public string ClientSecret { get; set; } = "123licensee1%^&sit";

        [BindProperty]
        public string Seq { get; set; } = "d8382c1d-6ce0-433d-9233-e7358b10a70b";


        public void OnGet()
        {
            ////try
            ////{
            ////    GenerateRedirecUrl().GetAwaiter().GetResult();
            ////}
            ////catch(Exception ex)
            ////{
            ////    ErrorMessage = ex.ToString();
            ////}

            ////if (SingleSignOnResult.IsSuccessful())
            ////{
            ////    // return singleSignOnResult.RedirectUri;
            ////}
            ////else
            ////{
            ////    // Handle Error
            ////}
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                SingleSignOnResult = await SingleSignOnAsync();

                if (SingleSignOnResult.IsSuccessful())
                {
                    return Redirect(SingleSignOnResult.RedirectUri.ToString());
                }
                else
                {
                    return Page();
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.ToString());
            }
        }

        private async Task<SingleSignOnResult> SingleSignOnAsync()
        {
            var authenticationInfo = new AuthenticationInfo(
                            apiUri: new Uri(appSettings.ApiUrl),
                            authUri: new Uri(appSettings.AuthUrl),
                            clientId: ClientId,
                            clientSecret: ClientSecret);

            var client = new AuthenticationClient(authenticationInfo);

            var signInParams = new SingleSignOnParams(
                siteId: authenticationInfo.SiteId,
                memberId: MemberId,
                memberName: MemberName,
                language: Language,
                currency: Currency,
                iP: Request.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                browserUserAgent: Request.Headers["User-Agent"].ToString(),
                seq: Seq);

            return await client.SingleSignOnAsync(signInParams);

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