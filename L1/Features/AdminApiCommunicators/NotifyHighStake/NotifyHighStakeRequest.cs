using System.Collections.Generic;

namespace L1.Features.AdminApiCommunicators.NotifyHighStake
{
    public class NotifyHighStakeRequest : AdminApiRequest
    {
        public NotifyHighStakeRequest(
            string obCustId,
            byte gameTypeId,
            decimal amount,
            string currency,
            int choiceId,
            byte system,
            Dictionary<string, string> metadata,
            string operatorId,
            string userId)
        {
            ObCustId = obCustId;
            GameTypeId = gameTypeId;
            Amount = amount;
            Currency = currency;
            ChoiceId = choiceId;
            System = system;
            Metadata = metadata;
            OperatorId = operatorId;
            UserId = userId;
        }

        public string ObCustId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public int ChoiceId { get; set; }

        public byte System { get; set; }

        public string OperatorId { get; set; }

        public string UserId { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}