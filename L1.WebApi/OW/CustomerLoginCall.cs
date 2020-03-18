

namespace L1.WebApi.OW
{
    using L1.Features.OWCommunicators;

    public class CustomerLoginCall : OWCall
    {
        public CustomerLoginCall(int obCustId, string seq, string characcterName, bool firstLogin)
            : base(seq, obCustId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
        }

        public string CharacterName { get; set; }

        public bool FirstLogin { get; set; }
    }
}
