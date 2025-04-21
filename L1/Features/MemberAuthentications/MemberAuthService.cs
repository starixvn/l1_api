using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L1.Features.GameConfigurations;
using L1.Features.Partners;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using L1.Shared.Helpers;
using L1.Shared.Json;
using L1.Shared.Resources;
using Sentry;

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

        private void LogToSentry(string message, VerificationParams verificationParams, MemberAuth memberAuth)
        {
            var logMessage =
                $"Verification failed on {DateTimeOffset.UtcNow}. " +
                $"Reason: {message}. " +
                $"Request: {jsonService.Serialize(verificationParams)}. " +
                $"Member Auth: {jsonService.Serialize(memberAuth)}";

            sentryClient.CaptureMessage(logMessage, SentryLevel.Warning);
        }

        private static string GetRootDomain(string domain)
        {
            var uri = new Uri(domain);
            var parts = uri.Host.Split('.');
            if (parts.Length <= 2)
            {
                return uri.Host;
            }
            return string.Join('.', parts.Skip(1));
        }

        private Uri GetDirectClientUrl(string redirectUrl, int gameId)
        {
            var url = appSettings.MicrositeClientPatternDomain.Replace("{host}", GetRootDomain(redirectUrl));
            var uri = new Uri(url);
            return new Uri(uri, $"{uri.LocalPath.TrimEnd('/')}/site-{gameId}/");
        }

        private async Task<Uri> GetClientUrl(MemberAuth memberAuth, bool shouldUsePortraitOrientation)
        {
            var partner = await partnerDataService.GetById(memberAuth.OperatorId);

            if (partner == null)
            {
                return null;
            }

            var clientUrl = memberAuth.AppType switch
            {
                AppTypes.SabaClub => shouldUsePortraitOrientation ? partner.MobileDirectUrl : partner.ClubVersionUrl,
                AppTypes.MiniSabaClub => partner.MiniVersionUrl,
                AppTypes.SabaMobi => partner.MobiVersionUrl,
                AppTypes.MegaKeno => partner.MegaKenoUrl,
                AppTypes.MobileDirect => partner.MobileDirectUrl,
                _ => partner.ClubVersionUrl,
            };

            return !string.IsNullOrEmpty(clientUrl) ? new Uri(clientUrl) : null;
        }

        private Uri GetRedirectedUrl(Uri clientUrl, int? gameId, bool isOpenDirectly, bool shouldUsePortraitOrientation)
        {
            if (shouldUsePortraitOrientation)
            {
                return clientUrl;
            }
            
            var gameClientUrl = clientUrl?.ToString();
            if (isOpenDirectly && !string.IsNullOrEmpty(gameClientUrl))
            {
                return GetDirectClientUrl(gameClientUrl.TrimEnd('/'), gameId.Value);
            }

            return clientUrl;
        }

        private async Task<GameConfigurationResponse> GetGameConfigurationAsync(int? gameId)
        {
            if (gameId > 0)
            {
                return await configurationApiService.GetDetailGameConfiguration(gameId.Value);
            }
            return null;
        }

        private static bool IsUsePortraitOrientation(MemberAuth memberAuth, GameConfigurationResponse gameConfiguration)
        {
            const string SABA_PLAY_OPERATOR_ID = "SabaPlay";
            var deviceType = DeviceTypeHelper.GetDeviceType(memberAuth.BrowserUserAgent);
            System.Diagnostics.Debug.WriteLine(memberAuth.OperatorId, gameConfiguration?.IsSupportedPortraitMode, deviceType);
            return memberAuth.OperatorId == SABA_PLAY_OPERATOR_ID
               && gameConfiguration?.IsSupportedPortraitMode == true
               && deviceType.Equals(DeviceType.Mobile);
        }

        public async Task<SingleSignOnResult> UpdateMemberAuth(UpdateMemberAuthParams updateParams)
        {
            var memberAuth = await memberAuthDataAccess.GetForTestCurrency(updateParams.Token);

            if (memberAuth == null)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.Unauthorize);
            }
            var gameConfiguration = await GetGameConfigurationAsync(updateParams.GameTypeId);
            var shouldUsePortraitOrientation = IsUsePortraitOrientation(memberAuth, gameConfiguration);
            var isOpenDirectly = memberAuth.AppType == AppTypes.SabaClub
                && gameConfiguration?.IsMicrosite == true
                && gameConfiguration?.IsOpenDirectly == true;
            var gameClientUrl = await GetClientUrl(memberAuth, shouldUsePortraitOrientation);
            var redirectUrl = GetRedirectedUrl(gameClientUrl, updateParams.GameTypeId, isOpenDirectly, shouldUsePortraitOrientation);

            if (redirectUrl == null)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.GameServerNotFoundCode);
            }

            await memberAuthDataAccess.UpdateCurrencyGroup(updateParams.Token, updateParams.CurrencyGroupId);

            var optionalUrlParams = new OptionalURLParams
            {
                GameId = updateParams.GameTypeId,
                IsStandalone = isOpenDirectly,
                LobbyUrl = isOpenDirectly ? gameClientUrl?.ToString().TrimEnd('/') : null,
            };

            return CreateSSOResult(
                redirectUrl.ToString(),
                memberAuth.Token,
                memberAuth.Language,
                optionalUrlParams);
        }

        private static SingleSignOnResult CreateSSOResult(string redirectUrl, string token, string language, OptionalURLParams optionalURLParams)
        {
            var clientRedirectUrl = new StringBuilder();
            clientRedirectUrl.Append($"{redirectUrl}?token={token}&lang={language}");

            var gameId = optionalURLParams.GameId;
            var isStandalone = optionalURLParams.IsStandalone;
            var lobbyUrl = optionalURLParams.LobbyUrl;

            if (gameId.HasValue)
            {
                clientRedirectUrl.Append($"&gameId={gameId}");
            }

            if (isStandalone)
            {
                clientRedirectUrl.Append($"&isStandalone=true");
            }

            if (!string.IsNullOrEmpty(lobbyUrl))
            {
                var lobbyUrlEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(lobbyUrl));
                clientRedirectUrl.Append($"&lurl={lobbyUrlEncoded}");
            }
            return SingleSignOnResult.CreateSuccessResponse(clientRedirectUrl.ToString());
        }
    }
    class OptionalURLParams
    {
        public int? GameId { get; set; }
        public bool IsStandalone { get; set; }
        public string LobbyUrl { get; set; }
    }
}