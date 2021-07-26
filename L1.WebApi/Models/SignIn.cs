using L1.Shared.Models;

namespace L1.WebApi.Models
{
    public class SignIn : Request
    {
        public string Domain { get; set; }

        public string BrowserUserAgent { get; set; }

        public string Currency { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public string MemberName { get; set; }     

        public byte? GameTypeId { get; set; }

        public bool IsMobi { get; set; }

        public string SiteName { get; set; }

        public Auth Auth { get; set; }
    }
}