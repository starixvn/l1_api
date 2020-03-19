namespace OW.SimulatorApi.Models.OW
{
    public class PlaceBetCall : OWCall
    {
        public int GameRoundId {get;set;}

        public int GameTypeId { get; set; }
        public int ActionId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }

    }
}
