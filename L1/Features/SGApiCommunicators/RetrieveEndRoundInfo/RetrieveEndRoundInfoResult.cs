using System;

namespace L1.Features.SGApiCommunicators
{
    public class RetrieveEndRoundInfoResult : SGResult
    {
        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public decimal ValidBetAmount { get; set; }

        public DateTimeOffset? EndTime { get; set; }
    }
}