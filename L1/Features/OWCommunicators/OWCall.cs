using System;
using L1.Features.Partners;

namespace L1.Features.OWCommunicators
{
    public class OWCall : PartnerCall
    {
        public OWCall(int obCustId, string siteId)
            : this(Guid.NewGuid().ToString(), obCustId, siteId)
        {
        }

        public OWCall(string seq, int obCustId, string siteId) : base(seq, obCustId, siteId)
        {
            Seq = seq ?? throw new ArgumentNullException(nameof(seq));
            SiteId = siteId ?? throw new ArgumentNullException(nameof(siteId));
            ObCustId = obCustId;
        }
    }
}