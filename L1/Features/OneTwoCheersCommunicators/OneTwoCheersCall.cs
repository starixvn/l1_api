namespace L1.Features.OneTwoCheersCommunicators
{
    using System;
    using L1.Features.PartnerCommunicators;
    public class OneTwoCheersCall : PartnerCall
    {
        public OneTwoCheersCall(string playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public OneTwoCheersCall(string seq, string playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}
