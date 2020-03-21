namespace L1.Features.OWCommunicators.PlaceBet
{
    public class PlaceBetCall : OWCall
    {
        public PlaceBetCall(string seq, int obCustId, string siteId, int gameRoundId, int gameTypeId, int choiceId, decimal amount, string currency, string ip) : base(seq, obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            ChoiceId = choiceId;
            Amount = amount;
            Currency = currency;
            Ip = ip;
        }

        public int GameRoundId { get; }

        public int GameTypeId { get; }

        public int ChoiceId { get; }

        public decimal Amount { get; }

        public string Currency { get; }

        public string Ip { get; }
    }
}