namespace L1.Features.PartnerCommunicators.EnterPortal
{
    public class PartnerEnterPortalCall : PartnerCall
    {
        public PartnerEnterPortalCall(string seq, int obCustId, string siteId, string characcterName, bool firstLogin)
          : base(seq, obCustId, siteId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
        }

        public string CharacterName { get; set; }

        public bool FirstLogin { get; set; }
    }
}