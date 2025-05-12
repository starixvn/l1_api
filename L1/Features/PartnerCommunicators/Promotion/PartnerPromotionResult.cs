using System;

namespace L1.Features.PartnerCommunicators.Promotion
{
    public class PartnerPromotionResult : PartnerResult
    {
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
} 