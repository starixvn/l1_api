namespace L1.Features.Authentications
{
    public interface IMemberAuthService
    {

    }

    public class MemberAuthService : IMemberAuthService
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;

        public MemberAuthService(IMemberAuthDataAccess memberAuthDataAccess)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
        }
    }
}