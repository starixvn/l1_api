using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Award;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.HitJackpot;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Features.PFFCommunicators;
using L1.Features.SboCommunicators;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using Polly;
using Sentry;

namespace L1.Features.SabaPlayCommunicators
{
    public class SabaPlayCustomerService : IPartnerCustomerServices
    {
        private readonly ISabaPlayService sabaPlayService;
        private readonly ILogService logService;
        private readonly SabaPlayEndPoints endpoints;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IAdminApiService adminApiSerivce;

        public SabaPlayCustomerService(
            ISabaPlayService SabaPlayService,
            ILogService logService,
            IAdminApiService adminApiSerivce,
            SabaPlayEndPoints endpoints,
            ISentryClient sentryClient,
            IAppSettings appSettings)
        {
            this.sabaPlayService = SabaPlayService;
            this.logService = logService;
            this.endpoints = endpoints;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.adminApiSerivce = adminApiSerivce;
        }

        public string OperatorId { get; set; }

        public async Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        {
            var response = await sabaPlayService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);
            if (response.Result.IsSuccessful() && appSettings.LogEnabling.EnterPortal)
            {
                await logService.LogEnterPortalAsync(OperatorId, response);
            }

            return response;
        }

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => sabaPlayService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);

        public async Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        {
            var response = await sabaPlayService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);

            if (response.Result.IsSuccessful() && appSettings.LogEnabling.GetBetLimit)
            {
                await logService.LogGetBetLimitAsync(OperatorId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub, Dictionary<string, string> metadata = null)
        {
            var response = await sabaPlayService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(appSettings.PartnerIds.SabaPlay, response, system);

                if (call.Amount >= appSettings.HighStake)
                {
                    var notifyHighStakeData = new NotifyHighStakeData(
                        SabaPlayFormatter.FormatPlayerId(call.SiteId, call.PlayerId.ToString()),
                        call.GameTypeId,
                        call.Amount,
                        call.Currency,
                        call.ChoiceId,
                        system,
                        metadata,
                        call.OperatorId,
                        call.UserId);

                    await adminApiSerivce.NotifyHighStake(new NotifyHighStakeRequest(notifyHighStakeData));
                }
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await sabaPlayService.Post<PartnerEndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(appSettings.PartnerIds.SabaPlay, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await sabaPlayService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(appSettings.PartnerIds.SabaPlay, response);
            }

            return response;
        }

        public async Task<PartnerResponse<HitJackpotResult>> HitJackpot(HitJackpotCall call)
        {
            const double retryDelayInSeconds = 30;
            const int numberOfRetries = 6;

            var response = await Policy
                .HandleResult<PartnerResponse<HitJackpotResult>>(response => response.Result.IsFailed())
                .WaitAndRetryAsync(numberOfRetries, retryAttempt => TimeSpan.FromSeconds(retryDelayInSeconds))
                .ExecuteAsync(() => sabaPlayService.Post<HitJackpotResult>(endpoints.HitJackpot, call));

            if (response.Result.IsSuccessful())
            {
                await logService.LogHitJackpotAsync(appSettings.PartnerIds.SabaPlay, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call HitJackpot failed for Partner {appSettings.PartnerIds.SabaPlay} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerAwardResult>> Award(PartnerAwardCall call)
        {
            var response = await sabaPlayService.Post<PartnerAwardResult>(endpoints.Award, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogAwardAsync(OperatorId, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call Award failed for Partner {appSettings.PartnerIds.SabaPlay} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }
    }
}