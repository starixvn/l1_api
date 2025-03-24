using System.Collections.Generic;

namespace L1.Features.AdminApiCommunicators.NotifyHighStake
{
    public record NotifyHighStakeData(
        string ObCustId,
        int GameTypeId,
        decimal Amount,
        string Currency,
        int ChoiceId,
        byte System,
        Dictionary<string, string> Metadata,
        string OperatorId,
        string UserId);

    public class NotifyHighStakeRequest : AdminApiRequest
    {
        public NotifyHighStakeRequest(NotifyHighStakeData data)
        {
            ObCustId = data.ObCustId;
            GameTypeId = data.GameTypeId;
            Amount = data.Amount;
            Currency = data.Currency;
            ChoiceId = data.ChoiceId;
            System = data.System;
            Metadata = data.Metadata;
            OperatorId = data.OperatorId;
            UserId = data.UserId;
        }

        public string ObCustId { get; set; }

        public int GameTypeId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public int ChoiceId { get; set; }

        public byte System { get; set; }

        public string OperatorId { get; set; }

        public string UserId { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}