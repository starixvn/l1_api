namespace L1.Features.PartnerCommunicators.GetBalance
{
    public class PartnerGetBalanceCall : PartnerCall
    {
        public PartnerGetBalanceCall(int playerId, string siteId, string operatorToken) : base(playerId, siteId)
        {
            OperatorToken = operatorToken;
        }

        public string OperatorToken { get; set; }
    }
}