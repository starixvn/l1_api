namespace L1.Features.AdminApiCommunicators.AddSpins
{
    public class AddSpinRequest : AdminApiRequest
    {
        public AddSpinRequest(int obCustId, byte spins)
        {            
            ObCustId = obCustId;
            Spins = spins;
        }

        public int ObCustId { get; }

        public byte Spins { get; }
    }
}
