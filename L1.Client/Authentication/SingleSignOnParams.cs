namespace L1.Client
{
    public class SingleSignOnParams
    {
        private const string DefaultLanguageCode = "vi-VN";

        public SingleSignOnParams(
            string clientId,
            string clientSecret,
            string siteId,
            int memberId,
            string memberName,
            string currency,
            string iP,
            string browserUserAgent,
            string seq)
            : this(
                  clientId,
                  clientSecret,
                  siteId,
                  memberId,
                  memberName,
                  DefaultLanguageCode,
                  currency,
                  iP,
                  browserUserAgent,
                  seq)
        {
        }

        public SingleSignOnParams(
            string clientId,
            string clientSecret,
            string siteId,
            int memberId,
            string memberName,
            string language,
            string currency,
            string iP,
            string browserUserAgent,
            string seq)
        {
            Auth = new Auth
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            SiteId = siteId;
            MemberId = memberId;
            MemberName = memberName;
            Language = language;
            Currency = currency;
            IP = iP;
            BrowserUserAgent = browserUserAgent;
            Seq = seq;
        }

        public string ClientId { get; }

        public string SiteId { get; }

        public int MemberId { get; }

        public string MemberName { get; }

        public string Language { get; }

        public string IP { get; }

        public string Currency { get; }

        public string BrowserUserAgent { get; }

        public string Seq { get; }

        public Auth Auth { get; }
    }

    public class Auth
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}