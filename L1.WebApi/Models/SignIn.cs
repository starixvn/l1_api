using L1.Shared.Constants;
using L1.Shared.Models;

namespace L1.WebApi.Models
{
    public class SignIn : Request
    {
        public string Domain { get; set; }

        public string BrowserUserAgent { get; set; }

        public string Currency { get; set; }

        public string Language { get; set; } = "en-US";

        public string IP { get; set; }

        public string MemberName { get; set; }     

        public byte? GameTypeId { get; set; }

        public byte AppType { get; set; } = AppTypes.SabaClub;

        public string SiteName { get; set; } 

        public bool IsCash { get; set; }

        public Auth Auth { get; set; }
    }
}