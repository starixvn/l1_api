namespace L1.Features.PartnerCommunicators.EnterPortal
{
    public class PartnerEnterPortalCall : PartnerCall
    {
        public PartnerEnterPortalCall(string seq, int playerId, string siteId, string characcterName, bool firstLogin)
          : base(seq, playerId, siteId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
        }

        public string CharacterName { get; set; }

        public bool FirstLogin { get; set; }
    }
}