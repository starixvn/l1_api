using L1.Features.AdminApiCommunicators;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Configurations;
using L1.Shared.Helpers;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace L1.Features.SKCommunicators.CheckMaintenance
{
    public interface ISKMaintenanceService
    {
        Task ObserveMaintenanceSchedule(OWRequest request, string operatorId);
    }

    public class SKMaintenanceService : PartnerMaintenanceService, ISKMaintenanceService
    {
        private const string HeaderAccessKey = "Saba-access-key";
        private readonly SKServiceSettings skServiceSettings;
        private readonly string secretKey;
        private readonly IAppSettings appSettings;

        public SKMaintenanceService(
            HttpClient httpClient,
            SKServiceSettings skServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IAppSettings appSettings)
            : base(httpClient, adminApiService, sentryClient)
        {
            this.skServiceSettings = skServiceSettings;
            secretKey = skServiceSettings.SecretKey;
            this.appSettings = appSettings;
        }

        public Task ObserveMaintenanceSchedule(OWRequest request, string operatorId)
        => ObserveMaintenanceSchedule(
                operatorId,
                new PartnerRequest(request.Call),
                new Uri(skServiceSettings.BaseUrl, skServiceSettings.Endpoints.CheckMaintenance));

        protected override async Task<PartnerMaintenanceResult> GetMaintenanceSchedule(string partnerId, PartnerRequest request, Uri maintenanceUri)
        {
            var checkingMaintenanceCall = new PartnerCall(request.Call.PlayerId, request.Call.SiteId);
            var skRequestBody = JsonConvert.SerializeObject(checkingMaintenanceCall);

            var content = new StringContent(skRequestBody, Encoding.UTF8, DefaultMediaType);
            var accessKey = Md5Helper.Create($"{secretKey}{skRequestBody}");

            httpClient.DefaultRequestHeaders.Add(HeaderAccessKey, accessKey);

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
    }
}