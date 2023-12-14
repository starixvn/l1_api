using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.PFFCommunicators
{
    public class PFFCall : PartnerCall
    {
        public PFFCall(string playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public PFFCall(string seq, string playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}