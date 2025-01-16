using System;
using System.Text;
using System.Threading.Tasks;
using L1.Features.Partners;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using L1.Shared.Resources;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.MemberAuthentications
{
    public interface IMemberAuthService
    {
        Task<VerificationResult> VerifyToken(VerificationParams verificationParams);

        Task<SingleSignOnResult> UpdateMemberAuth(UpdateMemberAuthParams updateParams);
    }

    public class MemberAuthService : IMemberAuthService
    {
        private readonly IMemberAuthDataAccess memberAuthDataAccess;
        private readonly IPartnerDataService partnerDataService;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;

        public MemberAuthService(
            IMemberAuthDataAccess memberAuthDataAccess,
            IPartnerDataService partnerDataService,
            IAppSettings appSettings,
            ISentryClient sentryClient)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
            this.partnerDataService = partnerDataService;
            this.appSettings = appSettings;
            this.sentryClient = sentryClient;
        }

        public async Task<VerificationResult> VerifyToken(VerificationParams verificationParams)
        {
            var memberAuth = await memberAuthDataAccess.GetByToken(verificationParams.Token);

            if (memberAuth != null)
            {
                if (memberAuth.BrowserUserAgent?.ToUpper() != verificationParams.BrowserUserAgent?.ToUpper())
                {
                    LogToSentry(VerifyTokenErrors.InvalidUserAgentOrIP, verificationParams, memberAuth);

                    return VerificationResult.CreateErrorResponse(VerificationResult.InvalidUserAgentOrIPCode);
                }

                if (memberAuth.Time.AddSeconds(appSettings.TokenExpiredSeconds) <= DateTime.Now)
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

        private void LogToSentry(string message, VerificationParams verificationParams, MemberAuth memberAuth)
        {
            sentryClient.CaptureMessage(
                $"Verification failed on {DateTime.UtcNow}. Reason: {message}. Request: {JsonConvert.SerializeObject(verificationParams)}. Member Auth: {JsonConvert.SerializeObject(memberAuth)}",
                SentryLevel.Warning);
        }

        private async Task<Uri> GetRedirectedUrl(MemberAuth memberAuth, bool isTestCurrency)
        {
            var partner = await partnerDataService.GetById(memberAuth.OperatorId);

            if (partner == null)
            {
                return null;
            }

            var redirectedUrl = isTestCurrency
                ? partner.TestCurrencyUrl
                : memberAuth.AppType switch
                {
                    AppTypes.SabaClub => partner.ClubVersionUrl,
                    AppTypes.MiniSabaClub => partner.MiniVersionUrl,
                    AppTypes.SabaMobi => partner.MobiVersionUrl,
                    AppTypes.MegaKeno => partner.MegaKenoUrl,
                    AppTypes.MobileDirect => partner.MobileDirectUrl,
                    _ => partner.ClubVersionUrl,
                };

            return string.IsNullOrWhiteSpace(redirectedUrl) ? null : new Uri(redirectedUrl);
        }

        public async Task<SingleSignOnResult> UpdateMemberAuth(UpdateMemberAuthParams updateParams)
        {
            var memberAuth = await memberAuthDataAccess.GetForTestCurrency(updateParams.Token);

            if (memberAuth == null)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.Unauthorize);
            }

            var gameClientUrl = await GetRedirectedUrl(memberAuth, false);

            if (gameClientUrl == null)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.GameServerNotFoundCode);
            }

            await memberAuthDataAccess.UpdateCurrencyGroup(updateParams.Token, updateParams.CurrencyGroupId);

            return CreateSSOResult(
                updateParams.GameTypeId,
                memberAuth.Token,
                memberAuth.Language,
                gameClientUrl.ToString().TrimEnd('/'));
        }

        private static SingleSignOnResult CreateSSOResult(int? gameTypeId, string token, string language, string gameClientUrl, string partnerId = null)
        {
            var clientRedirectUrl = new StringBuilder();
            clientRedirectUrl.Append($"{gameClientUrl}?token={token}&lang={language}");

            if (gameTypeId.HasValue)
            {
                clientRedirectUrl.Append($"&gameId={gameTypeId}");
            }

            if (!string.IsNullOrWhiteSpace(partnerId))
            {
                clientRedirectUrl.Append($"&partnerId={partnerId}");
            }

            return SingleSignOnResult.CreateSuccessResponse(clientRedirectUrl.ToString());
        }
    }
}