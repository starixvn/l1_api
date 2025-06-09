namespace L1.WebApi.Models
{
    public class HitJackpot : MemberRequest
    {
        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public decimal WinningAmount { get; set; }

        public string SiteName { get; set; }
    }
}