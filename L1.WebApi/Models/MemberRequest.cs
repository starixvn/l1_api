namespace L1.WebApi.Models
{
    public class MemberRequest
    {
        protected MemberRequest()
        {
        }

        public string OperatorId { get; set; }

        public string PlayerId { get; set; }

        public string SiteId { get; set; }
    }
}