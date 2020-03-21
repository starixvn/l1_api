namespace L1.WebApi.Models
{
    public abstract class MemberRequest
    {
        public int MemberId { get; set; }

        public string SiteId { get; set; }
    }
}