using System;
using L1.Features.Sites;
using L1.Shared.Configurations;
using L1.Shared.Resources;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

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
                if (memberAuth.BrowserUserAgent.ToUpper() != verificationParams.BrowserUserAgent?.ToUpper())
                {
                    LogToSentry(VerifyTokenErrors.InvalidUserAgentOrIP, verificationParams, memberAuth);
                    return VerificationResult.CreateErrorResponse(VerificationResult.UserIpNotFoundCode);
                }

                if (memberAuth.Time.AddSeconds(appSettings.TokenExpiredSeconds) <= DateTime.Now)
                {
                    return VerificationResult.CreateErrorResponse(VerificationResult.TokenExpiredCode);
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

            return VerificationResult.CreateErrorResponse(VerificationResult.TokenNotFoundCode);
        }

        private void LogToSentry(string message, VerificationParams verificationParams, MemberAuth memberAuth)
        {
            SentrySdk.CaptureMessage($"Verification failed on {DateTime.UtcNow}. Reason: {message}. Request: {JsonConvert.SerializeObject(verificationParams)}. Member Auth: {JsonConvert.SerializeObject(memberAuth)}", SentryLevel.Warning);
        }

        private string GetGameClientUrl(MemberAuth memberAuth)
        {
            var site = siteDataService.GetByClientId(memberAuth.ClientId);

            return site?.GameClientUrl;
        }
    }
}