namespace L1.WebApi.Models
{
    public class VoidGame : MemberRequest
    {
        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public string Reason { get; set; }
    }
}