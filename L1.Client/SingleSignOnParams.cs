namespace L1.Client
{
    public class SingleSignOnParams
    {
        public SingleSignOnParams(
            string siteId,
            int memberId,
            string memberName, 
            string language,
            string iP, 
            string browserUserAgent)
        {
            SiteId = siteId;
            MemberId = memberId;
            MemberName = memberName;
            Language = language;
            IP = iP;
            BrowserUserAgent = browserUserAgent;
        }

        public string SiteId { get; }

        public int MemberId { get; }

        public string MemberName { get; }

        public string Language { get; }

        public string IP { get; }

        public string BrowserUserAgent { get; }
    }
}