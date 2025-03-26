namespace L1.Features.PartnerCommunicators.GetBetLimit
{
    public class PartnerGetBetLimitCall : PartnerCall
    {
        public PartnerGetBetLimitCall(object playerId, string siteId, string operatorToken) : base(playerId, siteId)
        {
            OperatorToken = operatorToken;
        }

        public string OperatorToken { get; set; }
    }
}