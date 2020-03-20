namespace L1.Features.OWCommunicators.NotifyLogin
{
    public class NotifyLoginCall : OWCall
    {
        public NotifyLoginCall(string seq, int obCustId, string siteId, string characcterName, bool firstLogin)
          : base(seq, obCustId, siteId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
        }

        public string CharacterName { get; }

        public bool FirstLogin { get; }
    }
}