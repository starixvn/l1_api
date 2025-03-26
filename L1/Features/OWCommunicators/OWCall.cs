using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.OWCommunicators
{
    public class OWCall : PartnerCall
    {
        public OWCall(object playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public OWCall(string seq, object playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}