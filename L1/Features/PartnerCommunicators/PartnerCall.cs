using System;

namespace L1.Features.PartnerCommunicators
{
    public interface IPartnerCall
    {
        string Seq { get; set; }

        int PlayerId { get; set; }

        string SiteId { get; set; }
    }

    public class PartnerCall : IPartnerCall
    {
        public PartnerCall(int obCustId, string siteId)
            : this(Guid.NewGuid().ToString(), obCustId, siteId)
        {
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public PartnerCall(string seq, int obCustId, string siteId)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Seq = seq ?? throw new ArgumentNullException(nameof(seq));
            SiteId = siteId ?? throw new ArgumentNullException(nameof(siteId));
            PlayerId = obCustId;
        }

        public string Seq { get; set; }

        public int PlayerId { get; set; }

        public string SiteId { get; set; }

        public DateTimeOffset TimeStamp { get; }
    }
}