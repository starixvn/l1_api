namespace L1.WebApi.Models
{
    public class HitJackpot : MemberRequest
    {
        public long GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public decimal WinningAmount { get; set; }
    }
}