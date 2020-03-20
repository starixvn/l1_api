namespace L1.Features.OWCommunicators.GetBalance
{
    public class GetBalanceCall : OWCall
    {
        public GetBalanceCall(string seq, int obCustId, string siteId) : base(seq, obCustId, siteId)
        {
        }
    }
}