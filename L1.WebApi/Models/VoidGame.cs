namespace L1.WebApi.Models
{
    public class VoidGame : MemberRequest
    {
        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public string Reason { get; set; }
    }
}