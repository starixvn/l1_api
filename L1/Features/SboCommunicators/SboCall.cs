using L1.Features.Partners;
using System;

namespace L1.Features.SboCommunicators
{
    public class SboCall : PartnerCall
    {
        public SboCall(int obCustId, string siteId)
            : this(Guid.NewGuid().ToString(), obCustId, siteId)
        {
        }

        public SboCall(string seq, int obCustId, string siteId) : base(seq, obCustId, siteId)
        {
            Seq = seq ?? throw new ArgumentNullException(nameof(seq));
            SiteId = siteId ?? throw new ArgumentNullException(nameof(siteId));
            ObCustId = obCustId;
        }
    }
}