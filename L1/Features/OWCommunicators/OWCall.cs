using System;

namespace L1.Features.OWCommunicators
{
    public interface IOWCall
    {
        string Seq { get; }

        int ObCustId { get; }

        string SiteId { get; }
    }

    public class OWCall : IOWCall
    {
        public OWCall(int obCustId)
            : this(Guid.NewGuid().ToString(), obCustId, string.Empty)
        {
        }

        public OWCall(string seq, int obCustId, string siteId)
        {
            Seq = seq ?? throw new ArgumentNullException(nameof(seq));
            SiteId = siteId ?? throw new ArgumentNullException(nameof(SiteId));
            ObCustId = obCustId;
        }

        public string Seq { get; }

        public int ObCustId { get; }

        public string SiteId { get; }

    }
}