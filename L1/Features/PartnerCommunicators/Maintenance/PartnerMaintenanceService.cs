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

namespace L1.Features.PartnerCommunicators.Maintenance
{
    public interface IPartnerMaintenanceService
    {
        bool IsUnderMaintenance();

        Task ObserveMaintenanceSchedule(string partnerId, PartnerRequest request, Uri maintenanceUri);
    }

    public class PartnerMaintenanceService : IPartnerMaintenanceService
    {
        protected const string DefaultMediaType = "application/json";

        private int UnderMaintenanceCounter;

        protected readonly HttpClient httpClient;
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;

        public PartnerMaintenanceService(
            HttpClient httpClient,
            IAdminApiService adminApiService,
            ISentryClient sentryClient)
        {
            this.httpClient = httpClient;
            this.adminApiService = adminApiService;
            this.sentryClient = sentryClient;
        }

        public bool IsUnderMaintenance() => UnderMaintenanceCounter == 0;

        public async Task ObserveMaintenanceSchedule(string partnerId, PartnerRequest request, Uri maintenanceUri)
        {
            if (UnderMaintenanceCounter == 0)
            {
                Interlocked.Increment(ref UnderMaintenanceCounter);

                await CreateObservationPolicy().ExecuteAsync(() => GetMaintenanceSchedule(partnerId, request, maintenanceUri));
            }
        }

        protected virtual async Task<PartnerMaintenanceResult> GetMaintenanceSchedule(string partnerId, PartnerRequest request, Uri maintenanceUri)
        {
            var checkingMaintenanceCall = new PartnerCall(request.Call.PlayerId.ToString(), request.Call.SiteId);
            var owRequestBody = new PartnerRequest(checkingMaintenanceCall);
            var content = new StringContent(JsonConvert.SerializeObject(owRequestBody), Encoding.UTF8, DefaultMediaType);
            var response = await httpClient.PostAsync(maintenanceUri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var maintenanceScheduleResponse = JsonConvert.DeserializeObject<PartnerResponse<PartnerMaintenanceResult>>(responseContent);
                var maintenanceSchedule = maintenanceScheduleResponse.Result;

                await HandleMaintenanceSchedule(partnerId, maintenanceSchedule);

                return maintenanceSchedule;
            }

            return new PartnerMaintenanceResult();
        }

        protected async Task HandleMaintenanceSchedule(string partnerId, PartnerMaintenanceResult maintenanceSchedule)
        {
            var isUnderMaintenance = maintenanceSchedule.IsUnderMaintenance();

            if (!isUnderMaintenance)
            {
                Interlocked.Exchange(ref UnderMaintenanceCounter, 0);
            }

            sentryClient.CaptureMessage(
                $"Received maintenance schedule of {partnerId}: IsUM: {isUnderMaintenance} - {maintenanceSchedule.StartTime} - {maintenanceSchedule.EndTime}");

            await adminApiService.UnderMaintenance(new AdminApi.MaintenanceRequest(
                maintenanceSchedule.StartTime,
                maintenanceSchedule.EndTime,
                isUnderMaintenance,
                partnerId));
        }

        private static AsyncRetryPolicy<PartnerMaintenanceResult> CreateObservationPolicy()
        {
            const double retryDelayInMilliseconds = 5000;
            const int numberOfRetries = 3;

            var retryDelay = TimeSpan.FromMilliseconds(retryDelayInMilliseconds);

            return Policy
                .HandleResult<PartnerMaintenanceResult>(maintenanceSchedule => maintenanceSchedule.IsUnderMaintenance())
                .WaitAndRetryForeverAsync(
                    (_, maintenanceSchedule, _) => retryDelay,
                    (_, _, _, _) =>
                    {
                        Console.WriteLine("Check Maintenance Call!!");
                        return Task.CompletedTask;
                    });
        }
    }
}