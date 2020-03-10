namespace L1.Client
{
    public class SingleSignOnParams
    {
        public SingleSignOnParams(string siteId, int memberId, string memberName)
        {
            SiteId = siteId;
            MemberId = memberId;
            MemberName = memberName;
        }

        public string SiteId { get; }

        public int MemberId { get; }

        public string MemberName { get; }
    }
}