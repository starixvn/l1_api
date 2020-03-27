using System;
using L1.Features.Sites;
using L1.Shared.Configurations;
using L1.Shared.Resources;
using Sentry;

namespace L1.Features.Authentications
{
    public interface IMemberAuthService
    {
        SingleSignOnResult SignIn(MemberAuth memberAuth, byte? gameTypeId);

        VerificationResult VerifyToken(VerificationParams verificationParams);
    }

    public class MemberAuthService : IMemberAuthService
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly ISiteDataService siteDataService;
        private readonly IAppSettings appSettings;

        public MemberAuthService(
            IMemberAuthDataAccess memberAuthDataAccess,
            ISiteDataService siteDataService,
            IAppSettings appSettings)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
            this.siteDataService = siteDataService;
            this.appSettings = appSettings;
        }

        public SingleSignOnResult SignIn(MemberAuth memberAuth, byte? gameTypeId)
        {
            var gameClientUrl = GetGameClientUrl(memberAuth);

            if (string.IsNullOrWhiteSpace(gameClientUrl))
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.GameServerNotFoundCode);
            }

            var latestSignIn = memberAuthDataAccess.GetLatest(memberAuth);

            string token;

            if (latestSignIn != null && latestSignIn.Time.AddSeconds(appSettings.TokenExpiredSeconds) > DateTime.Now)
            {
                token = latestSignIn.Token;
            }
            else
            {
                memberAuth.Token = token = Guid.NewGuid().ToString();
                memberAuth.Time = DateTime.Now;
                memberAuthDataAccess.Create(memberAuth);
            }

            if (gameTypeId.HasValue)
            {
                return SingleSignOnResult.CreateSuccessResponse($"{gameClientUrl.TrimEnd('/')}/?token={token}&gameId={gameTypeId.Value}");
            }

            return SingleSignOnResult.CreateSuccessResponse($"{gameClientUrl.TrimEnd('/')}/?token={token}");
        }

        public VerificationResult VerifyToken(VerificationParams verificationParams)
        {
            var memberAuth = memberAuthDataAccess.GetByToken(verificationParams.Token);

            if (memberAuth != null)
            {
                if (memberAuth.Time.AddSeconds(appSettings.TokenExpiredSeconds) <= DateTime.Now)
                {
                    SentrySdk.CaptureMessage(VerifyTokenErrors.TokenExpired);
                    return VerificationResult.CreateErrorResponse(VerificationResult.TokenExpiredCode);
                }

                if (memberAuth.BrowserUserAgent.ToUpper() != verificationParams.BrowserUserAgent?.ToUpper())
                {
                    SentrySdk.CaptureMessage(VerifyTokenErrors.UserDeviceNotFound);
                    return VerificationResult.CreateErrorResponse(VerificationResult.UserDeviceNotFoundCode);
                }


                if (memberAuth.IP != verificationParams?.IP)
                {
                    SentrySdk.CaptureMessage(VerifyTokenErrors.UserIpNotFound);
                    return VerificationResult.CreateErrorResponse(VerificationResult.UserIpNotFoundCode);
                }

                return VerificationResult.CreateSuccessResponse(
                    memberAuth.MemberId,
                    memberAuth.MemberName,
                    memberAuth.SiteId,
                    memberAuth.Language,
                    memberAuth.Currency,
                    memberAuth.Seq,
                    memberAuth.ClientId);
            }

            SentrySdk.CaptureMessage(VerifyTokenErrors.TokenNotFound);
            return VerificationResult.CreateErrorResponse(VerificationResult.TokenNotFoundCode);
        }

        private string GetGameClientUrl(MemberAuth memberAuth)
        {
            var site = siteDataService.GetByClientId(memberAuth.ClientId);

            return site?.GameClientUrl;
        }
    }
}