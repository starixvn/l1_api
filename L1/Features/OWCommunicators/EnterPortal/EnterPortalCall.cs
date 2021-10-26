using L1.Features.Partners.EnterPortal;

namespace L1.Features.OWCommunicators.EnterPortal
{
    public class EnterPortalCall : PartnerEnterPortalCall
    {
        public EnterPortalCall(string seq, int obCustId, string siteId, string characcterName, bool firstLogin)
          : base(seq, obCustId, siteId, characcterName, firstLogin)
        {
        }
    }
}