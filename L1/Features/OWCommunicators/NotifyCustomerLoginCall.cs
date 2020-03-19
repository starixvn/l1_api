namespace L1.Features.OWCommunicators
{
    public class NotifyCustomerLoginCall : OWCall
    {
        public NotifyCustomerLoginCall(int obCustId, string seq, string siteId, string characcterName, bool firstLogin)
          : base(seq, obCustId, siteId)
        {
            CharacterName = characcterName;
            FirstLogin = firstLogin;
        }

        public string CharacterName { get;  }

        public bool FirstLogin { get;  }
    }
}
