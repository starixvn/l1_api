using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Helpers;
using Sentry;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Shared.Json;

namespace L1.Features.SGCommunicators.CheckMaintenance
{
    public interface ISGMaintenanceService
    {
        Task ObserveMaintenanceSchedule(SGRequest request, string operatorId);
    }

    public class SGMaintenanceService : PartnerMaintenanceService, ISGMaintenanceService
    {
        private const string HeaderAccessKey = "Saba-access-key";
        private readonly SGServiceSettings sgServiceSettings;
        private readonly string secretKey;
        private readonly IJsonService jsonService;

        public SGMaintenanceService(
            HttpClient httpClient,
            SGServiceSettings sgServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IJsonService jsonService)
            : base(httpClient, adminApiService, sentryClient, jsonService)
        {
            this.sgServiceSettings = sgServiceSettings;
            this.secretKey = sgServiceSettings.SecretKey;
            this.jsonService = jsonService;
        }

        public Task ObserveMaintenanceSchedule(SGRequest request, string operatorId)
        => ObserveMaintenanceSchedule(
                operatorId,
                new PartnerRequest(request.Call),
                new Uri(sgServiceSettings.BaseUrl, sgServiceSettings.Endpoints.CheckMaintenance));

        protected override async Task<PartnerMaintenanceResult> GetMaintenanceSchedule(string partnerId, PartnerRequest request, Uri maintenanceUri)
        {
            var checkingMaintenanceCall = new PartnerCall(request.Call.PlayerId, request.Call.SiteId);
            var sgRequestBody = jsonService.Serialize(checkingMaintenanceCall);

            var content = new StringContent(sgRequestBody, Encoding.UTF8, DefaultMediaType);
            var accessKey = Md5Helper.Create($"{secretKey}{sgRequestBody}");

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