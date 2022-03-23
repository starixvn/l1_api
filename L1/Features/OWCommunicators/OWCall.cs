using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.OWCommunicators
{
    public class OWCall : PartnerCall
    {
        public OWCall(int playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public OWCall(string seq, int playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}