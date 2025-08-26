using L1.Features.PartnerCommunicators.Award;

namespace L1.Features.SGCommunicators.Award
{
    public class SGAwardCall : PartnerAwardCall
    {
        public SGAwardCall(int playerId, string siteId, int gameTypeId, decimal amount, string currency, string promotionId, byte awardType) 
            : base(playerId, siteId, gameTypeId, amount, promotionId, currency, awardType)
        {
        }
    }
} 