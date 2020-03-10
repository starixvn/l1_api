using System;
using System.Collections.Generic;
using System.Linq;
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

        public SingleSignOnResult singleSignOnResult { get; private set; }

        public async Task OnGet()
        {
            var authenticationInfo = new AuthenticationInfo(
                new Uri("http://localhost:8868/"), 
                new Uri("http://localhost:8898/"), 
                "bwing3-client", 
                "bwing3-secret", 
                "bwing3");

            var client = new AuthenticationClient(authenticationInfo);

            var signInParams = new SingleSignOnParams("1", 343434, "membername");

            singleSignOnResult = await client.SingleSignOnAsync(signInParams);
        }
    }
}
