using System;

namespace L1.Features.PartnerCommunicators.Promotion
{
    public class PartnerStartPromotionCall : PartnerCall
    {
        public PartnerStartPromotionCall(string playerId, string siteId, string promotionCode, DateTime start, DateTime end)
            : base(playerId, siteId)
        {
            PromotionCode = promotionCode;
            Start = start;
            End = end;
        }

        public string PromotionCode { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
} 