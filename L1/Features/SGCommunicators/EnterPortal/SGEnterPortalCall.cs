using L1.Features.PartnerCommunicators.EnterPortal;

namespace L1.Features.SGCommunicators.EnterPortal
{
    public class SGEnterPortalCall : PartnerEnterPortalCall
    {
        public SGEnterPortalCall(int playerId, string siteId, string operatorToken)
            : base(playerId, siteId, "", false, operatorToken)
        {
        }
    }
} 