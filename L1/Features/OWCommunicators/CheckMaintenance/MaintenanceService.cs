using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using L1.Shared.Configurations;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.OWCommunicators.CheckMaintenance
{
    public interface IMaintenanceService
    {
        Task ObserveMaintenanceSchedule(OWRequest request);
    }

    public class MaintenanceService : IMaintenanceService
    {
        private const string DefaultMediaType = "application/json";

        private static int UnderMaintenanceCounter = 0;

        private readonly HttpClient httpClient;
        private readonly OWServiceSettings owServiceSettings;
        private readonly GameServerSetting gameServerSetting;
        private readonly AdminApiSettings adminApiSettings;

        public MaintenanceService(HttpClient httpClient, IAppSettings appSettings)
        {
            this.httpClient = httpClient;

            owServiceSettings = appSettings.OWServiceSettings;
            gameServerSetting = appSettings.GameServerSettings;
            adminApiSettings = appSettings.AdminApiSettings;
        }

        public async Task ObserveMaintenanceSchedule(OWRequest request)
        {
            if (UnderMaintenanceCounter == 0)
            {
                Interlocked.Increment(ref UnderMaintenanceCounter);

                await CreateObservationPolicy().ExecuteAsync(() => GetMaintenanceSchedule(request));
            }
        }

        private async Task<MaintenanceResult> GetMaintenanceSchedule(OWRequest request)
        {
            var checkingMaintenanceCall = new OWCall(request.Call.ObCustId, request.Call.SiteId);
            var owRequestBody = new OWRequest(null, checkingMaintenanceCall);
            var content = new StringContent(JsonConvert.SerializeObject(owRequestBody), Encoding.UTF8, DefaultMediaType);
            var url = new Uri(owServiceSettings.BaseUrl, owServiceSettings.Endpoints.CheckMaintenance);
            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var maintenanceScheduleResponse = JsonConvert.DeserializeObject<OWResponse<MaintenanceResult>>(responseContent);
                var maintenanceSchedule = maintenanceScheduleResponse.Result;

                await HandleMaintenanceSchedule(maintenanceSchedule);

                return maintenanceSchedule;
            }

            return new MaintenanceResult();
        }

        private async Task HandleMaintenanceSchedule(MaintenanceResult maintenanceSchedule)
        {
            var isUnderMaintenance = maintenanceSchedule.IsUnderMaintenance();
            var maintenanceNotification = isUnderMaintenance ? new MaintenanceNotification(true) : new MaintenanceNotification(false);

            if (isUnderMaintenance)
            {
                SentrySdk.CaptureMessage(
                    $"Maintenance schedule: {maintenanceSchedule.StartTime} - {maintenanceSchedule.EndTime}",
                    SentryLevel.Info);
            }
            else
            {
                Interlocked.Exchange(ref UnderMaintenanceCounter, 0);
            }

            await SendMaintenanceNotification(maintenanceNotification, new Uri(gameServerSetting.BaseUrl, gameServerSetting.Endpoints.UnderMaintenance));
            await SendMaintenanceNotification(maintenanceNotification, new Uri(adminApiSettings.BaseUrl, adminApiSettings.Endpoints.UnderMaintenance));
        }

        private async Task SendMaintenanceNotification(MaintenanceNotification notification, Uri endpoint)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, DefaultMediaType);

                await httpClient.PostAsync(endpoint, content);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage(ex.ToString(), SentryLevel.Error);
            }
        }

        private static AsyncRetryPolicy<MaintenanceResult> CreateObservationPolicy()
        {
            const double retryDelayInMilliseconds = 100;
            const int numberOfRetries = 3;

            var retryDelay = TimeSpan.FromMilliseconds(retryDelayInMilliseconds);

            return Policy
                .HandleResult<MaintenanceResult>(maintenanceSchedule => maintenanceSchedule.IsUnderMaintenance())
                .WaitAndRetryAsync(
                    numberOfRetries,
                    (retryCount, maintenanceSchedule, context) =>
                    {
                        var duration = maintenanceSchedule.Result.CalculateDuration();

                        return duration.Add(retryDelay);
                    },
                    (maintenanceSchedule, sleepDuration, retryCount, context) =>
                    {
                        return Task.CompletedTask;
                    });
        }
    }
}