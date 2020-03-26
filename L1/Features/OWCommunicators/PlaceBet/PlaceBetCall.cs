namespace L1.Features.OWCommunicators.PlaceBet
{
    public class PlaceBetCall : OWCall
    {
        public PlaceBetCall(int obCustId, string siteId, int gameRoundId, byte gameTypeId, int choiceId, decimal amount, string currency, string ip) : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            ChoiceId = choiceId;
            Amount = amount;
            Currency = currency;
            Ip = ip;
        }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public int ChoiceId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Ip { get; set; }
    }
}