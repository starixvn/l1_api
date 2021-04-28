namespace L1.Features.AdminApiCommunicators.NotifyHighStake
{
    public class NotifyHighStakeRequest : AdminApiRequest
    {
        public NotifyHighStakeRequest(int obCustId, byte gameTypeId, decimal amount, string currency)
        {
            ObCustId = obCustId;
            GameTypeId = gameTypeId;
            Amount = amount;
            Currency = currency;
        }

        public int ObCustId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }
    }
}