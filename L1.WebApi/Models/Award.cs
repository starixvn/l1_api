namespace L1.WebApi.Models
{
    public class Award : MemberRequest
    {
        public int GameTypeId { get; set; }

        public decimal Amount { get; set; }

        public int PromotionId { get; set; }
    }
}
