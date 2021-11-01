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
        Task<SingleSignOnResult> SignIn(MemberAuth memberAuth, byte? gameTypeId);

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

        public async Task<SingleSignOnResult> SignIn(MemberAuth memberAuth, byte? gameTypeId)
        {
            var currency = await partnerDataService.GetCurrency(memberAuth.PartnerId, memberAuth.CurrencyName);

            if (currency == null)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.Unauthorize);
            }

            var gameClientUrl = await GetRedirectedUrl(memberAuth, currency.IsTest);

            if (gameClientUrl == null)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.GameServerNotFoundCode);
            }

            var latestSignIn = await memberAuthDataAccess.GetLatest(memberAuth);

            string token;

            if (latestSignIn != null && latestSignIn.Time.AddSeconds(appSettings.TokenExpiredSeconds) > DateTime.Now)
            {
                token = latestSignIn.Token;
            }
            else
            {
                token = Guid.NewGuid().ToString();
                memberAuth.Token = token;
                memberAuth.Time = DateTime.Now;
                memberAuth.CurrencyCode = currency.Code;
                memberAuth.CurrencyGroupId = currency.GroupId;
                memberAuth.Multiplier = currency.Multiplier;
                memberAuth.IsTestCurrency = currency.IsTest;

                await memberAuthDataAccess.Create(memberAuth);
            }

            return CreateSSOResult(gameTypeId, token, memberAuth.Language, gameClientUrl.ToString().TrimEnd('/'));
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
                    memberAuth.MemberId,
                    memberAuth.MemberName,
                    memberAuth.SiteId,
                    memberAuth.Language,
                    memberAuth.CurrencyName,
                    memberAuth.Seq,
                    memberAuth.PartnerId,
                    memberAuth.Domain,
                    memberAuth.SiteName,
                    memberAuth.AppType,
                    memberAuth.CurrencyCode,
                    memberAuth.Multiplier,
                    memberAuth.CurrencyGroupId
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
            var partner = await partnerDataService.GetById(memberAuth.PartnerId);

            if (partner == null)
            {
                return null;
            }

            if (isTestCurrency)
            {
                return new Uri(partner.TestCurrencyUrl);
            }

            return memberAuth.AppType switch
            {
                AppTypes.SabaClub => new Uri(partner.ClubVersionUrl),
                AppTypes.MiniSabaClub => new Uri(partner.MiniVersionUrl),
                AppTypes.SabaMobi => new Uri(partner.MobiVersionUrl),
                _ => new Uri(partner.ClubVersionUrl),
            };
        }

        public async Task<SingleSignOnResult> UpdateMemberAuth(UpdateMemberAuthParams updateParams)
        {
            var memberAuth = await memberAuthDataAccess.GetByToken(updateParams.Token);

            if (memberAuth == null || !memberAuth.IsTestCurrency)
            {
                return SingleSignOnResult.CreateErrorResponse(SingleSignOnResult.Unauthorize);
            }

            await memberAuthDataAccess.UpdateCurrencyGroup(updateParams.Token, updateParams.CurrencyGroupId);

            var gameClientUrl = (await GetRedirectedUrl(memberAuth, false)).ToString().TrimEnd('/');

            return CreateSSOResult(updateParams.GameTypeId, memberAuth.Token, memberAuth.Language, gameClientUrl);
        }

        private static SingleSignOnResult CreateSSOResult(byte? GameTypeId, string token, string language, string gameClientUrl)
        {
            var clientRedirectUrl = new StringBuilder();
            clientRedirectUrl.Append($"{gameClientUrl}?token={token}&lang={language}");

            if (GameTypeId.HasValue)
            {
                clientRedirectUrl.Append($"&gameId={GameTypeId}");
            }

            return SingleSignOnResult.CreateSuccessResponse(clientRedirectUrl.ToString());
        }
    }
}