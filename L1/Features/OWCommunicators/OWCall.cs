using System;

namespace L1.Features.OWCommunicators
{
    public interface IOWCall
    {
        string Seq { get; }

        int ObCustId { get; }
    }

    public class OWCall : IOWCall
    {
        public OWCall(int obCustId)
            : this(Guid.NewGuid().ToString(), obCustId)
        {
        }

        public OWCall(string seq, int obCustId)
        {
            Seq = seq ?? throw new ArgumentNullException(nameof(seq));
            ObCustId = obCustId;
        }

        public string Seq { get; }

        public int ObCustId { get; }
    }
}