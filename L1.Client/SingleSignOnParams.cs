namespace L1.Client
{
    public class SingleSignOnParams
    {
        public SingleSignOnParams(
            string siteId,
            int memberId,
            string memberName,
            string language,
            string currency,
            string iP,
            string browserUserAgent,
            string owToken,
            string owSeq)
        {
            SiteId = siteId;
            MemberId = memberId;
            MemberName = memberName;
            Language = language;
            Currency = currency;
            IP = iP;
            BrowserUserAgent = browserUserAgent;
            OWToken = owToken;
            OWSeq = owSeq;
        }

        public string SiteId { get; }

        public int MemberId { get; }

        public string MemberName { get; }

        public string Language { get; }

        public string IP { get; }

        public string Currency { get; }

        public string BrowserUserAgent { get; }

        public string OWToken { get; }

        public string OWSeq { get; }
    }
}