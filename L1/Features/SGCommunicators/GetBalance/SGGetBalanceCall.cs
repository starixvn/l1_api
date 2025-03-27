using L1.Features.PartnerCommunicators.GetBalance;

namespace L1.Features.SGCommunicators.GetBalance
{
    public class SGGetBalanceCall : PartnerGetBalanceCall
    {
        public SGGetBalanceCall(int playerId, string siteId, string operatorToken) 
            : base(playerId, siteId, operatorToken)
        {
        }
    }
} 