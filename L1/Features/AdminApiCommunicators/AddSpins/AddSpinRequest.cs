namespace L1.Features.AdminApiCommunicators.AddSpins
{
    public class AddSpinRequest : AdminApiRequest
    {
        public AddSpinRequest(string key, int obCustId, byte spins)
        {
            Key = key;
            ObCustId = obCustId;
            Spins = spins;
        }

        public string Key { get; }

        public int ObCustId { get; }

        public byte Spins { get; }
    }
}
