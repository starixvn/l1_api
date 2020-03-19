

namespace L1.WebApi.OW
{
    using L1.Features.OWCommunicators;

    public class CustomerLoginCall : OWCall
    {
        public CustomerLoginCall(int obCustId, string seq, string characcterName, bool firstLogin, string siteId)
            : base(seq, obCustId, siteId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
        }

        public string CharacterName { get; }


        public bool FirstLogin { get; }
    }
}
