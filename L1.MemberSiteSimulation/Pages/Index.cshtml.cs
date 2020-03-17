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

        public string Currency { get; set; } = "UUS";

        public string ErrorMessage { get; set; }

        public string ClientId { get; set; } = "licensee1";

        public string ClientSecret { get; set; } = "123licensee1%^&sit";


        public string OWToken { get; set; } 

        public string OWSeq { get; set; }


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
                MemberId = int.Parse(Request.Form[nameof(MemberId)]);
                MemberName = Request.Form[nameof(MemberName)];
                Language = Request.Form[nameof(Language)];

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
            catch(Exception ex)
            {
                return new JsonResult(ex.ToString());
            }
        }

        private async Task<SingleSignOnResult> SingleSignOnAsync()
        {
            var authenticationInfo = new AuthenticationInfo(
                            //apiUri: new Uri("http://api.lumigame.com/"),
                             //apiUri: new Uri("http://10.253.14.11:8868"),
                             apiUri: new Uri("http://localhost:8868/"),
                            //authUri: new Uri("http://10.253.14.11:8898/"),
                            //authUri: new Uri("http://auth.lumigame.com/"),
                            authUri: new Uri("http://localhost:8898/"),
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
                owToken:OWToken,
                owSeq: OWSeq);

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