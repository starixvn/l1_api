using System;

namespace L1.Features.Partners
{
    public class PromotionDto
    {
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public PromotionStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
} 