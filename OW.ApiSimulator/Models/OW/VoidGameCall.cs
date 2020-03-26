namespace OW.ApiSimulator.Models.OW
{
    public class VoidGameCall : OWCall
    {
        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public string Reason { get; set; }
    }
}