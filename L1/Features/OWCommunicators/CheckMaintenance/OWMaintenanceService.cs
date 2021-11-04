using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Sentry;
using AdminApi = L1.Features.AdminApiCommunicators.UnderMaintenance;

namespace L1.Features.OWCommunicators.CheckMaintenance
{
    public interface IOWMaintenanceService
    {
        Task<bool> IsUnderMaintenance();

        Task ObserveMaintenanceSchedule(OWRequest request);
    }

    public class OWMaintenanceService : IOWMaintenanceService
    {
        private const string DefaultMediaType = "application/json";

        private static int UnderMaintenanceCounter;

        private readonly HttpClient httpClient;
        private readonly OWServiceSettings owServiceSettings;
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;

        public OWMaintenanceService(
            HttpClient httpClient,
            OWServiceSettings owServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient)
        {
            this.httpClient = httpClient;
            this.owServiceSettings = owServiceSettings;
            this.adminApiService = adminApiService;
            this.sentryClient = sentryClient;
        }

        public Task<bool> IsUnderMaintenance()
            => Task.FromResult(UnderMaintenanceCounter == 0);

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
            var owRequestBody = new OWRequest(checkingMaintenanceCall);
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

            if (!isUnderMaintenance)
            {
                Interlocked.Exchange(ref UnderMaintenanceCounter, 0);
            }

            sentryClient.CaptureMessage(
                $"Received maintenance schedule: IsUM: {isUnderMaintenance} - {maintenanceSchedule.StartTime} - {maintenanceSchedule.EndTime}");

            await adminApiService.UnderMaintenance(new AdminApi.MaintenanceRequest(maintenanceSchedule.StartTime, maintenanceSchedule.EndTime, isUnderMaintenance));
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
                    (_, maintenanceSchedule, _) =>
                    {
                        var duration = maintenanceSchedule.Result.CalculateDuration();

                        return duration.Add(retryDelay);
                    },
                    (_, _, _, _) =>
                    {
                        return Task.CompletedTask;
                    });
        }
    }
}