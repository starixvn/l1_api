using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L1.Features.GameConfigurations;
using L1.Features.Partners;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using L1.Shared.Resources;
using Sentry;
using L1.Features.Json;

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
        private readonly IConfigurationApiService configurationApiService;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;
        private readonly IJsonService jsonService;

        public MemberAuthService(
            IMemberAuthDataAccess memberAuthDataAccess,
            IPartnerDataService partnerDataService,
            IConfigurationApiService configurationApiService,
            IAppSettings appSettings,
            ISentryClient sentryClient,
            IJsonService jsonService)
        {
            this.memberAuthDataAccess = memberAuthDataAccess;
            this.partnerDataService = partnerDataService;
            this.configurationApiService = configurationApiService;
            this.appSettings = appSettings;
            this.sentryClient = sentryClient;
            this.jsonService = jsonService;
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

                if (memberAuth.Time.AddSeconds(appSettings.TokenExpiredSeconds) <= DateTimeOffset.Now)
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
            var logMessage = 
                $"Verification failed on {DateTimeOffset.UtcNow}. " +
                $"Reason: {message}. " +
                $"Request: {jsonService.Serialize(verificationParams)}. " +
                $"Member Auth: {jsonService.Serialize(memberAuth)}";
            
            sentryClient.CaptureMessage(logMessage, SentryLevel.Warning);
        }

        private string GetRootDomain(string domain)
        {
            var uri = new Uri(domain);
            var parts = uri.Host.Split('.');
            if(parts.Length <= 2)
            {
                return uri.Host;
            }
            return string.Join('.',parts.Skip(1));
        }

        private string GetDirectClientUrl(string redirectUrl, int gameId)
        {
            var url = appSettings.MicrositeClientPatternDomain.Replace("{host}", GetRootDomain(redirectUrl));
            var uri = new Uri(url);
            return new Uri(uri, $"{uri.LocalPath.TrimEnd('/')}/site-{gameId}/").ToString();
        }

        private async Task<string> GetRedirectedUrl(MemberAuth memberAuth, int? gameId, bool isOpenDirectly)
        {
            var partner = await partnerDataService.GetById(memberAuth.OperatorId);

            if (partner == null)
            {
                return null;
            }

            var redirectedUrl = memberAuth.AppType switch
                {
                    AppTypes.SabaClub => partner.ClubVersionUrl,
                    AppTypes.MiniSabaClub => partner.MiniVersionUrl,
                    AppTypes.SabaMobi => partner.MobiVersionUrl,
                    AppTypes.MegaKeno => partner.MegaKenoUrl,
                    AppTypes.MobileDirect => partner.MobileDirectUrl,
                    _ => partner.ClubVersionUrl,
                };

            if (isOpenDirectly && !string.IsNullOrEmpty(redirectedUrl))
            {
                return GetDirectClientUrl(redirectedUrl.TrimEnd('/'), gameId.Value);
            }

            return redirectedUrl?.Trim()?.TrimEnd('/');
        }

        private async Task<GameConfigurationResponse> GetGameConfigurationAsync(int? gameId)
        {
            if(gameId > 0)
            {
                return await configurationApiService.GetDetailGameConfiguration(gameId.Value);
            }
            return null;
        }

        public async Task<SingleSignOnResult> UpdateMemberAuth(UpdateMemberAuthParams updateParams)
        {
            var memberAuth = await memberAuthDataAccess.GetForTestCurrency(updateParams.Token);

            if (memberAuth == null)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.Unauthorize);
            }
            var gameConfiguration = await GetGameConfigurationAsync(updateParams.GameTypeId);
            var isOpenDirectly = memberAuth.AppType == AppTypes.SabaClub 
                && gameConfiguration?.IsMicrosite == true 
                && gameConfiguration?.IsOpenDirectly == true;
            var gameClientUrl = await GetRedirectedUrl(memberAuth, updateParams.GameTypeId, isOpenDirectly);
            
            if (string.IsNullOrEmpty(gameClientUrl))
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.GameServerNotFoundCode);
            }

            await memberAuthDataAccess.UpdateCurrencyGroup(updateParams.Token, updateParams.CurrencyGroupId);

            return CreateSSOResult(
                updateParams.GameTypeId,
                memberAuth.Token,
                memberAuth.Language,
                gameClientUrl,
                isOpenDirectly,
                gameConfiguration?.Gateway?.Id);
        }

        private static SingleSignOnResult CreateSSOResult(int? gameTypeId, string token, string language, string gameClientUrl, bool isOpenDirectly, int? gatewayId)
        {
            var clientRedirectUrl = new StringBuilder();
            clientRedirectUrl.Append($"{gameClientUrl}?token={token}&lang={language}");

            if (gameTypeId.HasValue)
            {
                clientRedirectUrl.Append($"&gameId={gameTypeId}");
            }

            if (isOpenDirectly)
            {
                clientRedirectUrl.Append($"&isStandalone=true");
            }

            if (gatewayId > 0)
            {
                clientRedirectUrl.Append($"&gatewayId={gatewayId}");
            }
            return SingleSignOnResult.CreateSuccessResponse(clientRedirectUrl.ToString());
        }
    }
}