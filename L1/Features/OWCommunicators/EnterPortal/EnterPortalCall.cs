namespace L1.Features.OWCommunicators.EnterPortal
{
    public class EnterPortalCall : OWCall
    {
        public EnterPortalCall(string seq, int obCustId, string siteId, string characcterName, bool firstLogin)
          : base(seq, obCustId, siteId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
        }

        public string CharacterName { get; set; }

        public bool FirstLogin { get; set; }
    }
}