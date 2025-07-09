using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using L1.Shared.Json;
using L1.Shared.Resources;
using Sentry;

namespace L1.Features.MemberAuthentications
{
    public interface IMemberAuthService
    {
        Task<VerificationResult> VerifyToken(VerificationParams verificationParams);
    }

    public class MemberAuthService : IMemberAuthService
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;

        public MemberAuthService(
            IMemberAuthDataAccess memberAuthDataAccess,
            IAppSettings appSettings,
            ISentryClient sentryClient,
            IJsonService jsonService)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
            this.appSettings = appSettings;
            this.sentryClient = sentryClient;
            this.jsonService = jsonService;
        }

        public async Task<VerificationResult> VerifyToken(VerificationParams verificationParams)
        {
            var memberAuth = await memberAuthDataAccess.GetByToken(verificationParams.Token);

            if (memberAuth != null)
            {
                var userAgentVerificationResult = VerifyUserAgent(verificationParams, memberAuth);
                if(!userAgentVerificationResult.IsSuccessful)
                {
                    return userAgentVerificationResult;
                }

                if (new DateTimeOffset(memberAuth.Time, TimeSpan.Zero).AddSeconds(appSettings.TokenExpiredSeconds) <= DateTimeOffset.UtcNow)
                {
                    return VerificationResult.CreateErrorResponse(VerificationResult.TokenExpiredCode);
                }

                return VerificationResult.CreateSuccessResponse(
                    memberAuth.PlayerId,
                    memberAuth.PlayerName,
                    memberAuth.SiteId,
                    memberAuth.Language,
                    memberAuth.CurrencyName,
                    memberAuth.Seq,
                    memberAuth.OperatorId,
                    memberAuth.Domain,
                    memberAuth.SiteName,
                    memberAuth.AppType,
                    memberAuth.CurrencyCode,
                    memberAuth.Multiplier,
                    memberAuth.CurrencyGroupId,
                    memberAuth.IsTestCurrency,
                    memberAuth.DisableJackpotGames,
                    memberAuth.OperatorToken,
                    memberAuth.IsCash,
                    memberAuth.OperatorPlayerId,
                    memberAuth.DisableAward
                    );
            }

            return VerificationResult.CreateErrorResponse(VerificationResult.TokenNotFoundCode);
        }

        private VerificationResult VerifyUserAgent(VerificationParams verificationParams, MemberAuth memberAuth)
        {
            if(appSettings.IgnoreBrowserAgentCheck != null 
                && appSettings.IgnoreBrowserAgentCheck.TryGetValue(memberAuth.OperatorId, out var ignoreAppTypes)
                && ignoreAppTypes != null 
                && ignoreAppTypes.Contains(memberAuth.AppType))
            {
                return new VerificationResult(VerificationResult.SuccessCode, string.Empty);
            }

            if (memberAuth.BrowserUserAgent?.ToUpper() != verificationParams.BrowserUserAgent?.ToUpper())
            {
                LogToSentry(VerifyTokenErrors.InvalidUserAgentOrIP, verificationParams, memberAuth);

                return VerificationResult.CreateErrorResponse(VerificationResult.InvalidUserAgentOrIPCode);
            }

            return new VerificationResult(VerificationResult.SuccessCode, string.Empty);
        }

        private void LogToSentry(string message, VerificationParams verificationParams, MemberAuth memberAuth)
        {
            var logMessage =
                $"Verification failed on {DateTimeOffset.UtcNow}. " +
                $"Reason: {message}. " +
                $"Request: {jsonService.Serialize(verificationParams)}. " +
                $"Member Auth: {jsonService.Serialize(memberAuth)}";

            sentryClient.CaptureMessage(logMessage, SentryLevel.Warning);
        }
    }
}