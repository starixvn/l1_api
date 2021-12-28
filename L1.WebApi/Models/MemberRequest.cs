namespace L1.WebApi.Models
{
    public class MemberRequest
    {
        protected MemberRequest()
        {
        }

        public string PartnerId { get; set; }

        public int ObCustId { get; set; }

        public string SiteId { get; set; }
    }
}