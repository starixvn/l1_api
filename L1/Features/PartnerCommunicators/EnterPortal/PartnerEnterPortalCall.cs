namespace L1.Features.PartnerCommunicators.EnterPortal
{
    public class PartnerEnterPortalCall : PartnerCall
    {
        public PartnerEnterPortalCall(int playerId, string siteId, string characcterName, bool firstLogin, string operatorToken)
          : base(playerId, siteId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
            OperatorToken = operatorToken;
        }

        public string CharacterName { get; set; }

        public bool FirstLogin { get; set; }

        public string OperatorToken { get; set; }
    }
}