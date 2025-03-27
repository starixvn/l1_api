using L1.Features.PartnerCommunicators.GetBetLimit;

namespace L1.Features.SGCommunicators.GetBetLimit
{
    public class SGGetBetLimitCall : PartnerGetBetLimitCall
    {
        public SGGetBetLimitCall(int playerId, string siteId, string operatorToken) 
            : base(playerId, siteId, operatorToken)
        {
        }
    }
} 