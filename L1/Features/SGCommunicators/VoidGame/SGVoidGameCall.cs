using L1.Features.PartnerCommunicators.VoidGame;

namespace L1.Features.SGCommunicators.VoidGame
{
    public class SGVoidGameCall : PartnerVoidGameCall
    {
        public SGVoidGameCall(int playerId, string siteId, int gameRoundId, int gameTypeId, string reason, bool freeBet) 
            : base(playerId, siteId, gameRoundId, gameTypeId, reason, freeBet)
        {
        }
    }
} 