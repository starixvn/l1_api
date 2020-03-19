namespace OW.SimulatorApi.Models.OW
{
    public class VoidGameCall : OWCall
    {
        public int GameRoundId { get; set; }

        public int ObTransId { get; set; }

        public string Reason { get; set; }
    }
}
