namespace L1.Features.ChatbotApiCommunicators.NotifyHighStake
{
    public class NotifyHighStakeRequest : ChatbotApiRequest
    {
        public NotifyHighStakeRequest(decimal amount, byte gameTypeId)
        {
            Amount = amount;
            GameTypeId = gameTypeId;
        }

        public decimal Amount { get; set; }

        public byte GameTypeId { get; set; }
    }
}