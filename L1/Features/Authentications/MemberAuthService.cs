using System;
using System.Linq;
using L1.Shared.Configurations;
using L1.Shared.Models;

namespace L1.Features.Authentications
{
    public interface IMemberAuthService
    {
        SingleSignOnResult SignIn(MemberAuth memberAuth);

        VerificationResult VerifyToken(VerificationParams verificationParams);
    }

    public class MemberAuthService : IMemberAuthService
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly IAppSettings appSettings;

        public MemberAuthService(
            IMemberAuthDataAccess memberAuthDataAccess,
            IAppSettings appSettings)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
            this.appSettings = appSettings;
        }

        public SingleSignOnResult SignIn(MemberAuth memberAuth)
        {
            var GameClientUrl = GetGameClientUrl(memberAuth);

            if (string.IsNullOrWhiteSpace(GameClientUrl))
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.GameServerNotFoundCode);
            }

            memberAuth.Token = Guid.NewGuid().ToString();
            memberAuth.Time = DateTime.Now;

            memberAuthDataAccess.Create(memberAuth);

            return SingleSignOnResult.CreateSuccessResponse($"{GameClientUrl.TrimEnd('/')}/?token={memberAuth.Token}");
        }

        public VerificationResult VerifyToken(VerificationParams verificationParams)
        {
            var memberAuth = memberAuthDataAccess.GetByToken(verificationParams.Token);

            if(memberAuth == null)
            {
                return VerificationResult.CreateErrorResponse(VerificationResult.TokenNotFoundCode);
            }

            if(memberAuth.Time.AddSeconds(appSettings.TokenExpiredSeconds) <= DateTime.Now)
            {
                return VerificationResult.CreateErrorResponse(VerificationResult.TokenExpiredCode);
            }

            return VerificationResult.CreateSuccessResponse(memberAuth.MemberId, memberAuth.MemberName, memberAuth.SiteId, memberAuth.Language);
        }

        private string GetGameClientUrl(MemberAuth memberAuth)
        {
            var getLicenseeConfig = appSettings.LicenseeConfigs.FirstOrDefault(config => config.ClientId == memberAuth.SiteId);

            return getLicenseeConfig?.GameClientUrl;
        }
    }
}