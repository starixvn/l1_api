using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.HitJackpot;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using Polly;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.OWCommunicators
{
    public class OWCustomerService : IPartnerCustomerServices
    {
        private readonly IOWService oWService;
        private readonly ILogService logService;
        private readonly OWEndPoints endpoints;
        private readonly IAppSettings appSettings;
        private readonly IAdminApiService adminApiSerivce;
        private readonly ISentryClient sentryClient;

        public OWCustomerService(
            IOWService oWService,
            ILogService logService,
            IAppSettings appSettings,
            IAdminApiService adminApiSerivce,
            OWEndPoints endpoints,
            ISentryClient sentryClient)
        {
            this.oWService = oWService;
            this.logService = logService;
            this.endpoints = endpoints;
            this.appSettings = appSettings;
            this.adminApiSerivce = adminApiSerivce;
            this.sentryClient = sentryClient;
        }

        public Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        => oWService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => oWService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);

        public Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        => oWService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub, Dictionary<string, string> metadata = null)
        {
            var response = await oWService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(appSettings.PartnerIds.OW, response, system, metadata);

                if (call.Amount >= appSettings.HighStake)
                {
                    await adminApiSerivce.NotifyHighStake(new NotifyHighStakeRequest(
                        call.PlayerId,
                        call.GameTypeId,
                        call.Amount,
                        call.Currency,
                        call.ChoiceId,
                        system,
                        metadata));
                }
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await oWService.Post<PartnerEndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(appSettings.PartnerIds.OW, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await oWService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(appSettings.PartnerIds.OW, response);
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
                .ExecuteAsync(() => oWService.Post<HitJackpotResult>(endpoints.HitJackpot, call));

            if (response.Result.IsSuccessful())
            {
                await logService.LogHitJackpotAsync(appSettings.PartnerIds.OW, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call HitJackpot failed for Partner {appSettings.PartnerIds.OW} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }
    }
}