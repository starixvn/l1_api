using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.PFFCommunicators
{
    public class PFFCall : PartnerCall
    {
        public PFFCall(object playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public PFFCall(string seq, object playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}