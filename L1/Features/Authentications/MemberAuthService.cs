using System;
using L1.Shared.Models;

namespace L1.Features.Authentications
{
    public interface IMemberAuthService
    {
        SingleSignOnResult SignIn(MemberAuth memberAuth);
    }

    public class MemberAuthService : IMemberAuthService
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;

        public MemberAuthService(IMemberAuthDataAccess memberAuthDataAccess)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
        }

        public SingleSignOnResult SignIn(MemberAuth memberAuth)
        {
            memberAuth.Token = Guid.NewGuid().ToString();
            memberAuth.Time = DateTime.Now;

            memberAuthDataAccess.Create(memberAuth);

            return SingleSignOnResult.CreateSuccessResponse("http://demo.com");
        }
    }
}