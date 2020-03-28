namespace L1.WebApi.Models
{
    public abstract class MemberRequest
    {
        public int ObCustId { get; set; }

        public string SiteId { get; set; }
    }
}