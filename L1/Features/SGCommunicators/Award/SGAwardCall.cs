using L1.Features.PartnerCommunicators.Award;

namespace L1.Features.SGCommunicators.Award
{
    public class SGAwardCall : PartnerAwardCall
    {
        public SGAwardCall(int playerId, string siteId, int gameTypeId, decimal amount, string currency) 
            : base(playerId, siteId, gameTypeId, amount, 0, currency)
        {
        }
    }
} 