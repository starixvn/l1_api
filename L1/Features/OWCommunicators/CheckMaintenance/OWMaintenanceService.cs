using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Helpers;
using Sentry;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.Json;

namespace L1.Features.OWCommunicators.CheckMaintenance
{
    public interface IOWMaintenanceService
    {
        Task ObserveMaintenanceSchedule(OWRequest request, string operatorId);
    }

    public class OWMaintenanceService : PartnerMaintenanceService, IOWMaintenanceService
    {
        private const string HeaderAccessKey = "Saba-access-key";
        private readonly OWServiceSettings owServiceSettings;
        private readonly string secretKey;
        private readonly IJsonService jsonService;

        public OWMaintenanceService(
            HttpClient httpClient,
            OWServiceSettings owServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IJsonService jsonService)
            : base(httpClient, adminApiService, sentryClient, jsonService)
        {
            this.owServiceSettings = owServiceSettings;
            this.secretKey = owServiceSettings.SecretKey;
            this.jsonService = jsonService;
        }

        public Task ObserveMaintenanceSchedule(OWRequest request, string operatorId)
        => ObserveMaintenanceSchedule(
                operatorId,
                new PartnerRequest(request.Call),
                new Uri(owServiceSettings.BaseUrl, owServiceSettings.Endpoints.CheckMaintenance));

        protected override async Task<PartnerMaintenanceResult> GetMaintenanceSchedule(string partnerId, PartnerRequest request, Uri maintenanceUri)
        {
            var checkingMaintenanceCall = new PartnerCall(request.Call.PlayerId.ToString(), request.Call.SiteId);
            var owRequestBody = jsonService.Serialize(checkingMaintenanceCall);

            var content = new StringContent(owRequestBody, Encoding.UTF8, DefaultMediaType);
            var accessKey = Md5Helper.Create($"{secretKey}{owRequestBody}");

            httpClient.DefaultRequestHeaders.Add(HeaderAccessKey, accessKey);

            var response = await httpClient.PostAsync(maintenanceUri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var maintenanceScheduleResponse = jsonService.Deserialize<PartnerResponse<PartnerMaintenanceResult>>(responseContent);
                var maintenanceSchedule = maintenanceScheduleResponse.Result;

                await HandleMaintenanceSchedule(partnerId, maintenanceSchedule);

                return maintenanceSchedule;
            }

            return new PartnerMaintenanceResult();
        }
    }
}