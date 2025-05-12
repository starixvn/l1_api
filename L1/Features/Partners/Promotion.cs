using System;

namespace L1.Features.Partners
{
    public class Promotion
    {
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PartnerId { get; set; }
    }
} 