namespace L1.Features.AdminApiCommunicators.NotifyHighStake
{
    public class NotifyHighStakeRequest : AdminApiRequest
    {
        public NotifyHighStakeRequest(int obCustId, byte gameTypeId, decimal amount, string currency, int choiceId, byte system)
        {
            ObCustId = obCustId;
            GameTypeId = gameTypeId;
            Amount = amount;
            Currency = currency;
            ChoiceId = choiceId;
            System = system;
        }

        public int ObCustId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public int ChoiceId { get; set; }

        public byte System { get; set; }
    }
}